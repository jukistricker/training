using BankAccountSimulation.Domain.Enums;
using BankAccountSimulation.Models;
using BankAccountSimulation.Services.Interfaces;
using BankAccountSimulation.ViewModels;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankAccountSimulation.Controllers;

[Authorize]
public class AccountsController : Controller
{
    private readonly IValidator<CreateAccountViewModel> _createValidator;
    private readonly IValidator<LoginAccountViewModel> _loginValidator;
    private readonly IAccountsService _accountService;

    public AccountsController(
        IValidator<CreateAccountViewModel> createValidator,
        IValidator<LoginAccountViewModel> loginValidator,
        IAccountsService accountService)
    {
        _createValidator = createValidator;
        _loginValidator = loginValidator;
        _accountService = accountService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
    {
        var accounts = await _accountService.GetAllAccounts() ?? new List<AccountsViewModel>();
        return View(accounts);
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateAccountViewModel request)
    {
        var result = await _createValidator.ValidateAsync(request);
        if (!result.IsValid)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            return View(request);
        }

        var account = await _accountService.CreateAccount(request);
        if (!account.IsSuccess)
        {
            ModelState.AddModelError(account.ErrorKey ?? "", account.ErrorMessage);
            return View(request);
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()

    {

        return View();

    }



    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginAccountViewModel request)
    {
        var result = await _loginValidator.ValidateAsync(request);
        if (!result.IsValid)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            return View(request);
        }

        var operationResult = await _accountService.Login(request);

        if (!operationResult.IsSuccess)
        {
            ModelState.AddModelError(operationResult.ErrorKey ?? string.Empty, operationResult.ErrorMessage ?? "Unknown error");
            return View(request);
        }

        var account = await _accountService.GetAccountDetails(request.AccountNumber);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, account.AccountNumber),
            new Claim(ClaimTypes.Role, account.Role)
        };
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            claimsPrincipal);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Login", "Accounts");
    }

    [HttpGet]
    public async Task<IActionResult> Details()
    {
        string? currentAccountNumber = User.Identity.Name;
        ViewBag.CurrentAccount = await _accountService.GetAccountDetails(currentAccountNumber);
        if (ViewBag.CurrentAccount == null) return NotFound();
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        IEnumerable<AccountsViewModel> accounts = new List<AccountsViewModel>();

        if (userRole == "Admin")
        {

            accounts = await _accountService.GetAllAccounts() ?? new List<AccountsViewModel>();
        }

        return View(accounts);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Freeze(string accountNumber)
    {
        await _accountService.UpdateAccountStatus(accountNumber, AccountStatus.Frozen);
        if (User.IsInRole("Admin"))
            return RedirectToAction(nameof(Index));
        return RedirectToAction(nameof(Details));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unfreeze(string accountNumber)
    {
        await _accountService.UpdateAccountStatus(accountNumber, AccountStatus.Active);
        if (User.IsInRole("Admin"))
            return RedirectToAction(nameof(Index));
        return RedirectToAction(nameof(Details));
    }
}