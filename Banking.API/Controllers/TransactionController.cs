using Banking.Application.DTOs.Transaction;
using Banking.Application.Interfaces;
using Banking.Domain.Entities;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Banking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionController : ControllerBase
{
    private readonly IAccountRepository _accountRepository;

    private readonly ITransactionRepository _transactionRepository;

    public TransactionController(
        IAccountRepository accountRepository,
        ITransactionRepository transactionRepository)
    {
        _accountRepository = accountRepository;

        _transactionRepository = transactionRepository;
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer(
        TransferRequestDto request)
    {
        var sender = await _accountRepository
            .GetByAccountNumberAsync(
                request.SenderAccountNumber);

        var receiver = await _accountRepository
            .GetByAccountNumberAsync(
                request.ReceiverAccountNumber);

        if (sender == null || receiver == null)
        {
            return BadRequest("Invalid account number.");
        }

        if (sender.Balance < request.Amount)
        {
            return BadRequest("Insufficient balance.");
        }

        sender.Balance -= request.Amount;

        receiver.Balance += request.Amount;

        await _accountRepository.UpdateAccountAsync(sender);

        await _accountRepository.UpdateAccountAsync(receiver);

        // Save Transaction Record
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),

            SenderAccountId = sender.Id,

            ReceiverAccountId = receiver.Id,

            Amount = request.Amount,

            TransactionType = "Transfer",

            Status = "Success",

            TransactionDate = DateTime.UtcNow
        };

        await _transactionRepository
            .AddTransactionAsync(transaction);

        await _accountRepository.SaveChangesAsync();

        await _transactionRepository.SaveChangesAsync();

        return Ok(new
        {
            Message = "Transfer successful."
        });
    }

    [HttpGet("history/{accountNumber}")]
    public async Task<IActionResult> GetTransactionHistory(
        string accountNumber)
    {
        var account = await _accountRepository
            .GetByAccountNumberAsync(accountNumber);

        if (account == null)
        {
            return BadRequest("Account not found.");
        }

        var transactions =
            await _transactionRepository
                .GetTransactionsByAccountIdAsync(account.Id);

        var response = transactions.Select(t => new TransactionHistoryDto
        {
            SenderAccountNumber = t.SenderAccount.AccountNumber,

            ReceiverAccountNumber = t.ReceiverAccount.AccountNumber,

            Amount = t.Amount,

            TransactionType = t.TransactionType,

            Status = t.Status,

            TransactionDate = t.TransactionDate
        });

        return Ok(response);
    }
}