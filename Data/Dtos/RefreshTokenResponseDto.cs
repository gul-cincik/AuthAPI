using System.IdentityModel.Tokens.Jwt;

namespace AuthAPI.Data.Dtos
{
    public class RefreshTokenResponseDto
    {
        public bool IsSuccess { get; set; }
        public string? Message {  get; set; } 
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ValidTo { get; set; }
    }
}
