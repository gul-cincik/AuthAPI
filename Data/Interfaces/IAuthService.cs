using AuthAPI.Data.Dtos;

namespace AuthAPI.Data.Interfaces
{
    public interface IAuthService
    {
        public Task<ServiceResponseDto> Register(RegisterDto registerDto);
        public Task<RefreshTokenResponseDto> Login(UserLoginDto registerDto);
        public Task<RefreshTokenResponseDto> RefreshToken(TokenDto tokenDto);
    }
}
