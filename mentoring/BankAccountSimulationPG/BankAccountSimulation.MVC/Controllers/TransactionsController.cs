using BankAccountSimulation.Domain.Enums;
using BankAccountSimulation.Services.Interfaces;
using BankAccountSimulation.ViewModels;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankAccountSimulation.Controllers;

[Authorize]
public class TransactionsController : Controller
{
    private readonly ITransactionsService _transactionService;
    private readonly IValidator<TransactionViewModel> _transactionValidator;
    private readonly IValidator<TransferViewModel> _transferValidator;

    public TransactionsController(ITransactionsService transactionService, IValidator<TransactionViewModel> transactionValidator, IValidator<TransferViewModel> transferValidator)
    {
        _transactionService = transactionService;
        _transactionValidator = transactionValidator;
        _transferValidator = transferValidator;
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
        var validationResult = await _transactionValidator.ValidateAsync(model);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            return View(model);
        }

        var result = await _transactionService.ProcessTransaction(model.AccountNumber, model.Amount, TransactionType.Deposit, model.Description??"Deposit Money");

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Deposit success!";
            return RedirectToAction("History", "Transactions");
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
        var validationResult = await _transactionValidator.ValidateAsync(model);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            return View(model);
        }

        var result = await _transactionService.ProcessTransaction(
            model.AccountNumber,
            model.Amount,
            TransactionType.Withdraw,
            model.Description ?? "Withdraw money"
        );

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Withdraw Success!";
            return RedirectToAction("History", "Transactions");
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
        var operationResult = await _transferValidator.ValidateAsync(model);
        if (!operationResult.IsValid)
        {
            foreach (var error in operationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            return View(model);
        }

        var result = await _transactionService.Transfer(model.SourceAccountNumber, model.DestinationAccountNumber, model.Amount, model.Description);

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Transfer success!";
            return RedirectToAction("History", "Transactions");
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