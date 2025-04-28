using PortfolioMVC.Models.DTOs;

namespace PortfolioMVC.Service
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<IEnumerable<ProductDto>> GetApprovedProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<ProductDto> CreateProductAsync(ProductDto productDto);
        Task<bool> UpdateProductAsync(int id, ProductDto productDto);
        Task<bool> DeleteProductAsync(int id);
        Task<bool> ApproveProductAsync(int id);
        Task<IEnumerable<ProductDto>> GetProductsByUserIdAsync(string userId);
    }
}