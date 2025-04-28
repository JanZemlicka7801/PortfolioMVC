using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioMVC.Service;

[Authorize]
public class OrdersController : Controller
{
    private readonly IOrderService _orderService;
    private readonly ICartService _cartService;

    public OrdersController(IOrderService orderService, ICartService cartService)
    {
        _orderService = orderService;
        _cartService = cartService;
    }

    // GET: Orders
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var orders = await _orderService.GetUserOrdersAsync(userId);
        return View(orders);
    }

    // GET: Orders/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var order = await _orderService.GetOrderDetailsAsync(id, userId);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    // GET: Orders/Checkout
    public async Task<IActionResult> Checkout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var cartId = await _cartService.GetCartIdAsync(userId);
        var cartItems = await _cartService.GetCartItemsAsync(cartId);
        var cartTotal = await _cartService.GetCartTotalAsync(cartId);

        if (!cartItems.Any())
        {
            return RedirectToAction("Index", "ShoppingCart");
        }

        ViewBag.CartItems = cartItems;
        ViewBag.CartTotal = cartTotal;

        return View();
    }

    // POST: Orders/CreateOrder
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateOrder()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var cartId = await _cartService.GetCartIdAsync(userId);
        var cartItems = await _cartService.GetCartItemsAsync(cartId);

        if (!cartItems.Any())
        {
            return RedirectToAction("Index", "ShoppingCart");
        }

        try
        {
            var order = await _orderService.CreateOrderAsync(userId, cartId);
            return RedirectToAction(nameof(OrderConfirmation), new { orderId = order.Id });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error creating order: {ex.Message}");
            ViewBag.CartItems = cartItems;
            ViewBag.CartTotal = await _cartService.GetCartTotalAsync(cartId);
            return View("Checkout");
        }
    }

    // GET: Orders/OrderConfirmation/5
    public async Task<IActionResult> OrderConfirmation(int orderId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var order = await _orderService.GetOrderDetailsAsync(orderId, userId);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    // GET: Orders/ManageOrders 
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ManageOrders()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return View(orders);
    }

    // POST: Orders/UpdateStatus/5 
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int orderId, PortfolioMVC.Models.Enums.OrderStatus status)
    {
        await _orderService.UpdateOrderStatusAsync(orderId, status);
        return RedirectToAction(nameof(ManageOrders));
    }
}