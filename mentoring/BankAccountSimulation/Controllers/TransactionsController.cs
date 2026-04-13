using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BankAccountSimulation.Services.Interfaces;
using BankAccountSimulation.ViewModels;
using BankAccountSimulation.Models;

namespace BankAccountSimulation.Controllers;

[Authorize]
public class TransactionsController : Controller
{
    private readonly ITransactionsService _transactionService;

    public TransactionsController(ITransactionsService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet]
    public IActionResult Deposit(string AccountNumber)
    {
        string? currentAccountNumber = User.Identity.Name;
        return View(new TransactionViewModel { AccountNumber = currentAccountNumber });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deposit(TransactionViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _transactionService.ProcessTransaction(model.AccountNumber, model.Amount, TransactionType.Deposit, model.Description??"Deposit Money");

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Deposit success!";
            return RedirectToAction("Details", "Accounts");
        }

        ModelState.AddModelError("", result.ErrorMessage);
        return View(model);
    }

    [HttpGet]
    public IActionResult Withdraw()
    {
        string? currentAccountNumber = User.Identity.Name;
        return View(new TransactionViewModel { AccountNumber = currentAccountNumber });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Withdraw(TransactionViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _transactionService.ProcessTransaction(
            model.AccountNumber,
            model.Amount,
            TransactionType.Withdraw,
            model.Description ?? "Withdraw money"
        );

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Withdraw Success!";
            return RedirectToAction("Details", "Accounts");
        }

        ModelState.AddModelError("", result.ErrorMessage);
        return View(model);
    }
    
    [HttpGet]
    public IActionResult Transfer()
    {
        string? currentAccountNumber = User.Identity.Name;
        return View(new TransferViewModel { SourceAccountNumber = currentAccountNumber });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Transfer(TransferViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _transactionService.Transfer(model.SourceAccountNumber, model.DestinationAccountNumber, model.Amount, model.Description);

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Transfer success!";
            return RedirectToAction("Details", "Accounts");
        }

        ModelState.AddModelError("", result.ErrorMessage);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> History(string type = "All", int page = 1)
    {
        string? currentAccountNumber = User.Identity.Name;

        ViewBag.AccountNumber = currentAccountNumber;

        var (items, totalCount) = await _transactionService.GetHistory(currentAccountNumber, type, page);

        int pageSize = 10;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        ViewBag.TotalCount = totalCount;
        ViewBag.CurrentType = type;

        return View(items);
    }
}