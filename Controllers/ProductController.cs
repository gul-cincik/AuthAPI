using AuthAPI.Data.Dtos;
using AuthAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        [Authorize]
        [Route("AddNewProduct")]
        [HttpPost]
        public async Task<IActionResult> AddNewProduct(AddProductDto dto)
        {
            ServiceResponseDto result = await _productService.AddNewProduct(dto);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }
    }
}
