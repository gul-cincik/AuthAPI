using AuthAPI.Data;
using AuthAPI.Data.Dtos;
using AuthAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Services
{
    public class ProductService
    {
        private readonly AuthDbContext _authDbContext;

        public ProductService(AuthDbContext authDbContext)
        {
            _authDbContext = authDbContext;
        }

        public async Task<ServiceResponseDto> AddNewProduct(AddProductDto dto)
        {

            try
            {
                Product existedProduct = await _authDbContext.Products.SingleOrDefaultAsync(p => p.Name == dto.Value);

                if (existedProduct != null)
                {
                    return new ServiceResponseDto { IsSuccess = false, Message = "Product is already addded." };
                }

                Product product = new Product()
                {
                    Name = dto.Value,
                    CreatedAt = DateTime.Now,
                    IsDeleted = false,
                };

                await _authDbContext.AddAsync(product);
                await _authDbContext.SaveChangesAsync();

                if (product.Id == Guid.Empty)
                {
                    return new ServiceResponseDto { IsSuccess = false, Message = "Product could not be addded." };
                }

                return new ServiceResponseDto { IsSuccess = true, Message = "Product is addded." };
            }
            catch (Exception ex)
            {
                return new ServiceResponseDto { IsSuccess = false, Message = ex.Message };

            }

        }

        public async Task<List<Product>> GetAllProducts()
        {

            try
            {
                List<Product> products = await _authDbContext.Products.ToListAsync();

                return products;
            }
            catch (Exception ex)
            {
                return null;
            }

        }


    }
}
