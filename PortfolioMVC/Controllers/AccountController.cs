﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PortfolioMVC.Controllers.Views;
using PortfolioMVC.Models.entities;

namespace PortfolioMVC.Controllers;

// References: https://www.youtube.com/watch?v=B0_gM-wBlmE
public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
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
    public IActionResult Login()
    {
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