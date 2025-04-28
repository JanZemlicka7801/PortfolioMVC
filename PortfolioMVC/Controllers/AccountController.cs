using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PortfolioMVC.Controllers.Views;
using PortfolioMVC.Models.entities;
using PortfolioMVC.Models.Enums;

namespace PortfolioMVC.Controllers;

// References: https://www.youtube.com/watch?v=B0_gM-wBlmE
public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountController(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    /// <summary>
    /// Handles the registration of a new user.
    /// </summary>
    /// <param name="model">The view model containing user registration data such as email, username, password, and other required details.</param>
    /// <returns>
    /// Returns a View if the model state is invalid or if the registration process fails.
    /// Redirects to the "Home/Index" action upon successful user registration.
    /// </returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new AppUser
        {
            UserName = model.Email,
            Email = model.Email,
            Name = model.Name,
            Department = model.Department
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            // Add user to RegisteredUser role
            await _userManager.AddToRoleAsync(user, "RegisteredUser");

            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Login(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    /// <summary>
    /// Handles the login process for a user.
    /// </summary>
    /// <param name="model">The view model containing user login data such as email, password, and an optional remember me flag.</param>
    /// <param name="returnUrl">The URL to redirect the user to upon successful login. Defaults to the application's home page if not provided.</param>
    /// <returns>
    /// Returns a View if the login attempt fails or the model state is invalid.
    /// Redirects to the specified URL or the application's default page upon a successful login.
    /// </returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            return RedirectToLocal(returnUrl);
        }
        else
        {
            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }
    }

    /// <summary>
    /// Logs out the currently signed-in user by ending their session and clearing any authentication cookies.
    /// </summary>
    /// <returns>
    /// Redirects to the "Index" action of the "Home" controller upon successful logout.
    /// </returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    /// <summary>
    /// Initiates an external login request using the specified provider.
    /// </summary>
    /// <param name="provider">The name of the external authentication provider (e.g., "Google", "Microsoft").</param>
    /// <param name="returnUrl">The URL to redirect to after successful authentication.</param>
    /// <returns>A challenge result that initiates the external authentication flow.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ExternalLogin(string provider, string returnUrl = null)
    {
        // Request a redirect to the external login provider
        var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    /// <summary>
    /// Handles the callback from external authentication providers.
    /// </summary>
    /// <param name="returnUrl">The URL to redirect to after successful authentication.</param>
    /// <param name="remoteError">Error information from the external provider, if any.</param>
    /// <returns>Redirects to the appropriate page based on authentication result.</returns>
    [HttpGet]
    public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
    {
        returnUrl = returnUrl ?? Url.Content("~/");

        if (remoteError != null)
        {
            ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
            return RedirectToAction(nameof(Login));
        }


        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return RedirectToAction(nameof(Login));
        }


        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
        if (result.Succeeded)
        {
            return RedirectToLocal(returnUrl);
        }

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var name = info.Principal.FindFirstValue(ClaimTypes.Name);

        if (email != null)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new AppUser
                {
                    UserName = email,
                    Email = email,
                    Name = name ?? email.Split('@')[0],
                    Department = Department.It
                };

                var createResult = await _userManager.CreateAsync(user);
                if (createResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "RegisteredUser");

                    createResult = await _userManager.AddLoginAsync(user, info);
                    if (createResult.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }

                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            else
            {
                var addLoginResult = await _userManager.AddLoginAsync(user, info);
                if (addLoginResult.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToLocal(returnUrl);
                }
            }
        }


        ViewData["ReturnUrl"] = returnUrl;
        ViewData["LoginProvider"] = info.LoginProvider;
        return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email, Name = name });
    }

    /// <summary>
    /// Handles the confirmation of an external login after user provides additional required information.
    /// </summary>
    /// <param name="model">The view model containing additional user information.</param>
    /// <param name="returnUrl">The URL to redirect to after successful authentication.</param>
    /// <returns>Redirects to the appropriate page based on confirmation result.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
    {
        returnUrl = returnUrl ?? Url.Content("~/");

        if (ModelState.IsValid)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name,
                Department = model.Department
            };

            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "RegisteredUser");

                result = await _userManager.AddLoginAsync(user, info);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToLocal(returnUrl);
                }
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View(model);
    }

    /// <summary>
    /// Shows access denied page when user tries to access restricted resources.
    /// </summary>
    /// <returns>Access denied view.</returns>
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    /// <summary>
    /// Redirects the user to the specified local URL if the return URL is a valid local URL.
    /// If the return URL is null or not a local URL, redirects to the "Index" action of the "Home" controller.
    /// </summary>
    /// <param name="returnUrl">The return URL to redirect to after a successful action.</param>
    /// <returns>
    /// Returns a RedirectResult to the specified local URL if valid,
    /// or a RedirectToActionResult to the "Index" action of the "Home" controller.
    /// </returns>
    private IActionResult RedirectToLocal(string returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        else
        {
            return RedirectToAction("Index", "Home");
        }
    }
}