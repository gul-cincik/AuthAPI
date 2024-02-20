using AuthAPI.Data;
using AuthAPI.Data.Dtos;
using AuthAPI.Data.Interfaces;
using AuthAPI.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthAPI.Services
{
    public class AuthService : IAuthService
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<ServiceResponseDto> Register(RegisterDto dto)
        {

            try
            {

                AppUser existedUser = await _userManager.FindByEmailAsync(dto.Email);

                if (existedUser != null)
                {
                    return new ServiceResponseDto { IsSuccess = false, Message = "This email is already registered." };

                }

                AppUser appUser = new() 
                { 
                    Email = dto.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = dto.Email,
                    Name = dto.Name,
                    Surname = dto.Surname
                };

                var result = await _userManager.CreateAsync(appUser, dto.Password);

                if (!result.Succeeded)
                {
                    return new ServiceResponseDto { IsSuccess = false, Message = "The user could not be created." };

                }


                bool saveRole = await RoleAssign(appUser, dto.RoleName);

                if (!saveRole)
                {
                    return new ServiceResponseDto { IsSuccess = true, Message = "Role could not be saved." };

                }

                return new ServiceResponseDto { IsSuccess = true, Message = "User Created Successfully." };

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return new ServiceResponseDto { IsSuccess = false, Message = ex.Message };
            }

        }

        public async Task<RefreshTokenResponseDto> Login(UserLoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                return new RefreshTokenResponseDto { IsSuccess = false, Message = "Invalid username." };
            }

            if(!(await _userManager.CheckPasswordAsync(user, dto.Password)))
            {
                return new RefreshTokenResponseDto { IsSuccess = false, Message = "Invalid password." };
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));   
            }

            var token = GenerateToken(authClaims);
            var refreshToken = GenerateRefreshToken();

            _ = int.TryParse(_configuration["Jwt:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);

            await _userManager.UpdateAsync(user);

            RefreshTokenResponseDto responseDto = new RefreshTokenResponseDto
            {
                IsSuccess = true,
                RefreshToken = refreshToken,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ValidTo = token.ValidTo,
            };

            return responseDto;
        }

        public async Task<RefreshTokenResponseDto> RefreshToken(TokenDto dto)
        {

            try
            {
                string? accessToken = dto.AccessToken;
                string? refreshToken = dto.RefreshToken;

                var principal = GetPrincipalFromExpiredToken(accessToken);

                if(principal == null)
                {
                    return new RefreshTokenResponseDto { IsSuccess = false, Message = "Invalid access token or refresh token" };

                }

                string username = principal.Identity.Name;

                var user = await _userManager.FindByNameAsync(username);

                if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                {
                    return new RefreshTokenResponseDto { IsSuccess = false, Message = "Invalid access token or refresh token" };

                }

                var newAccessToken = GenerateToken(principal.Claims.ToList());
                var newRefreshToken = GenerateRefreshToken();

                user.RefreshToken = newRefreshToken;

                await _userManager.UpdateAsync(user);

                return new RefreshTokenResponseDto
                {
                    IsSuccess = true,
                    Token = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                    RefreshToken = newRefreshToken
                };

            }
            catch (Exception ex)
            {
                return new RefreshTokenResponseDto { IsSuccess = false, Message = ex.Message };

            }

        }

        /// <summary>
        /// Generates Token according to the claims of the user's roles.
        /// </summary>
        /// <param name="authClaims"></param>
        /// <returns></returns>
        private JwtSecurityToken GenerateToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            _ = int.TryParse(_configuration["Jwt:TokenValidityInMinutes"], out int tokenValidityInMinutes);

            var token = new JwtSecurityToken(
                expires: DateTime.Now.AddMinutes(tokenValidityInMinutes),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        /// <summary>
        /// Generates refresh token.
        /// </summary>
        /// <returns></returns>
        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var nrg = RandomNumberGenerator.Create();
            nrg.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }


        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        public async Task<bool> RoleAssign(AppUser user, string roleName)
        {
            try
            {
                var result = await _userManager.AddToRoleAsync(user, roleName);

                if (!result.Succeeded)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex) 
            {
                return false;
            }
        } 

        //private JwtSecurityToken CreateToken(List<Claim> authClaims)
        //{
        //    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        //    _ = int.TryParse(_configuration["Jwt:TokenValidityInMinutes"], out int tokenValidityInMinutes);

        //    var token = new JwtSecurityToken(
        //        expires: DateTime.Now.AddMinutes(tokenValidityInMinutes),
        //        claims: authClaims,
        //        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        //        );

        //    return token;
        //}

        //private async Task<string> GenerateJwtToken(AppUser user)
        //{
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

        //    _ = int.TryParse(_configuration["Jwt:TokenValidityInMinutes"], out int tokenValidityInMinutes);

        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new[]
        //        {
        //            new Claim(ClaimTypes.NameIdentifier, user.Id),
        //            new Claim(ClaimTypes.Name, user.Email),
        //            new Claim(ClaimTypes.Email, user.Email)
        //            // Add more claims as needed
        //        }),
        //        Expires = DateTime.UtcNow.AddMinutes(tokenValidityInMinutes), // Token expiration time
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //    };

        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    return tokenHandler.WriteToken(token);
        //}


    }
}
