using AuthAPI.Data.Dtos;
using AuthAPI.Entities;
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

        [Authorize(Roles = "Sales Manager")]
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

        [Authorize(Roles = "Sales Manager, Sales Advisor")]
        [Route("GetAllProducts")]
        [HttpGet]
        public async Task<IActionResult> AddNeGetAllProductswProduct()
        {
            List<Product> result = await _productService.GetAllProducts();

            if (result == null)
            {
                return BadRequest("An error occured.");
            }

            return Ok(result);
        }
    }
}
