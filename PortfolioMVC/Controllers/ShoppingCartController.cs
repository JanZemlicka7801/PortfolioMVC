using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using PortfolioMVC.Service;

public class ShoppingCartController : Controller
{
    private readonly ICartService _cartService;
    private readonly IProductService _productService;

    public ShoppingCartController(ICartService cartService, IProductService productService)
    {
        _cartService = cartService;
        _productService = productService;
    }

    // GET: ShoppingCart
    public async Task<IActionResult> Index()
    {
        var cartId = await GetCartIdAsync();
        var cartItems = await _cartService.GetCartItemsAsync(cartId);
        var cartTotal = await _cartService.GetCartTotalAsync(cartId);

        ViewBag.CartTotal = cartTotal;
        return View(cartItems);
    }

    // POST: ShoppingCart/AddToCart
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
    {
        var product = await _productService.GetProductByIdAsync(productId);
        if (product == null || !product.IsApproved || !product.IsAvailable)
        {
            return NotFound();
        }

        var cartId = await GetCartIdAsync();
        await _cartService.AddToCartAsync(cartId, productId, quantity);

        return RedirectToAction(nameof(Index));
    }

    // POST: ShoppingCart/UpdateQuantity
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
    {
        var cartId = await GetCartIdAsync();
        await _cartService.UpdateCartQuantityAsync(cartId, cartItemId, quantity);

        return RedirectToAction(nameof(Index));
    }

    // POST: ShoppingCart/RemoveFromCart
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveFromCart(int cartItemId)
    {
        var cartId = await GetCartIdAsync();
        await _cartService.RemoveFromCartAsync(cartId, cartItemId);

        return RedirectToAction(nameof(Index));
    }

    // POST: ShoppingCart/ClearCart
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ClearCart()
    {
        var cartId = await GetCartIdAsync();
        await _cartService.ClearCartAsync(cartId);

        return RedirectToAction(nameof(Index));
    }

    private async Task<string> GetCartIdAsync()
    {
        string cartId;

        if (User.Identity.IsAuthenticated)
        {
            cartId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
        else
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("CartId")))
            {
                cartId = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("CartId", cartId);
            }
            else
            {
                cartId = HttpContext.Session.GetString("CartId");
            }
        }

        return await _cartService.GetCartIdAsync(cartId);
    }
}