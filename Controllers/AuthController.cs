using AuthAPI.Data.Dtos;
using AuthAPI.Entities;
using AuthAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;


namespace AuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AuthService _authService;
        private readonly ProductService _productService;

        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, AuthService authService, ProductService productService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authService = authService;
            _productService = productService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            ServiceResponseDto result = await _authService.Register(dto);

            if(!result.IsSuccess)
            {
                return BadRequest("User could not be registered.");
            }

            return Ok("User Created Successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto model)
        {
            var result = await _authService.Login(model);

            if (result.IsSuccess == false)
            {
                return Unauthorized(":(");
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken(TokenDto tokenDto)
        {
            if(tokenDto == null)
            {
                return BadRequest("Invalid request");
            }

            RefreshTokenResponseDto response = await _authService.RefreshToken(tokenDto);

            if(response.IsSuccess)
            {
                return Ok(response);
            }
            
            return Unauthorized(response);

        }
    }
}
