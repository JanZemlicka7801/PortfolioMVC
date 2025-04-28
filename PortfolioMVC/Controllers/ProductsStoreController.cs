using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioMVC.Models.DTOs;
using PortfolioMVC.Service;

namespace PortfolioMVC.Controllers
{
    public class ProductsStoreController : Controller
    {
        private readonly IProductService _productService;

        public ProductsStoreController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: ProductsStore
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetApprovedProductsAsync();
            return View(products);
        }

        // GET: ProductsStore/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null || !product.IsApproved)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: ProductsStore/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProductsStore/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                productDto.CreatedById = userId;

                productDto.IsApproved = User.IsInRole("Admin");

                await _productService.CreateProductAsync(productDto);
                return RedirectToAction(nameof(MyProducts));
            }
            return View(productDto);
        }

        // GET: ProductsStore/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            if (product.CreatedById != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return View(product);
        }

        // POST: ProductsStore/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductDto productDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingProduct = await _productService.GetProductByIdAsync(id);

            if (existingProduct == null)
            {
                return NotFound();
            }

            if (existingProduct.CreatedById != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            if (id != productDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                productDto.IsApproved = existingProduct.IsApproved;
                productDto.CreatedById = existingProduct.CreatedById;

                var success = await _productService.UpdateProductAsync(id, productDto);
                if (!success)
                {
                    return NotFound();
                }

                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction(nameof(ManageProducts));
                }

                return RedirectToAction(nameof(MyProducts));
            }
            return View(productDto);
        }

        // GET: ProductsStore/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            if (product.CreatedById != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return View(product);
        }

        // POST: ProductsStore/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            if (product.CreatedById != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            await _productService.DeleteProductAsync(id);

            if (User.IsInRole("Admin"))
            {
                return RedirectToAction(nameof(ManageProducts));
            }

            return RedirectToAction(nameof(MyProducts));
        }

        // GET: ProductsStore/MyProducts
        [Authorize]
        public async Task<IActionResult> MyProducts()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var products = await _productService.GetProductsByUserIdAsync(userId);
            return View(products);
        }

        // GET: ProductsStore/ManageProducts (Admin only)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        // POST: ProductsStore/Approve/5 (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await _productService.ApproveProductAsync(id);
            return RedirectToAction(nameof(ManageProducts));
        }
    }
}