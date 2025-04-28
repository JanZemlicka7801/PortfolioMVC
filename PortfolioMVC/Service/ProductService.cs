using Microsoft.EntityFrameworkCore;
using PortfolioMVC.Data;
using PortfolioMVC.Models.DTOs;
using PortfolioMVC.Models.entities;

namespace PortfolioMVC.Service
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _context.Products.Include(p => p.CreatedBy).ToListAsync();
            return products.Select(MapToDto);
        }

        public async Task<IEnumerable<ProductDto>> GetApprovedProductsAsync()
        {
            var products = await _context.Products
                .Where(p => p.IsApproved && p.IsAvailable)
                .Include(p => p.CreatedBy)
                .ToListAsync();
            return products.Select(MapToDto);
        }

        public async Task<Dictionary<PortfolioMVC.Models.Enums.ProductCategory, int>> GetProductCategoryCountsAsync()
        {
            var products = await _context.Products
                .Where(p => p.IsApproved && p.IsAvailable)
                .ToListAsync();

            return products
                .GroupBy(p => p.Category)
                .ToDictionary(
                    g => g.Key,
                    g => g.Count()
                );
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.CreatedBy)
                .FirstOrDefaultAsync(p => p.Id == id);

            return product == null ? null : MapToDto(product);
        }

        public async Task<ProductDto> CreateProductAsync(ProductDto productDto)
        {
            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                ImageUrl = productDto.ImageUrl,
                IsAvailable = productDto.IsAvailable,
                Category = productDto.Category,
                IsApproved = productDto.IsApproved,
                CreatedById = productDto.CreatedById
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            productDto.Id = product.Id;
            return productDto;
        }

        public async Task<bool> UpdateProductAsync(int id, ProductDto productDto)
        {
            if (id != productDto.Id)
                return false;

            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.Price = productDto.Price;
            product.ImageUrl = productDto.ImageUrl;
            product.IsAvailable = productDto.IsAvailable;
            product.Category = productDto.Category;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ApproveProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            product.IsApproved = true;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByUserIdAsync(string userId)
        {
            var products = await _context.Products
                .Where(p => p.CreatedById == userId)
                .Include(p => p.CreatedBy)
                .ToListAsync();

            return products.Select(MapToDto);
        }

        private ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                IsAvailable = product.IsAvailable,
                Category = product.Category,
                IsApproved = product.IsApproved,
                CreatedById = product.CreatedById
            };
        }
    }
}