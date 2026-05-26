using Banking.Application.DTOs.Account;
using Banking.Application.Interfaces;
using Banking.Domain.Entities;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

namespace Banking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountController : ControllerBase
{
    private readonly IAccountRepository _accountRepository;

    public AccountController(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateAccount(
        CreateAccountRequestDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return Unauthorized();
        }

        var account = new Account
        {
            Id = Guid.NewGuid(),

            UserId = Guid.Parse(userId),

            AccountNumber = GenerateAccountNumber(),

            AccountType = request.AccountType,

            Balance = 0
        };

        await _accountRepository.AddAccountAsync(account);

        await _accountRepository.SaveChangesAsync();

        return Ok(new
        {
            Message = "Account created successfully.",
            AccountNumber = account.AccountNumber
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetMyAccounts()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return Unauthorized();
        }

        var accounts = await _accountRepository
            .GetAccountsByUserIdAsync(Guid.Parse(userId));

        var response = accounts.Select(a => new AccountResponseDto
        {
            Id = a.Id,

            AccountNumber = a.AccountNumber,

            Balance = a.Balance,

            AccountType = a.AccountType
        });

        return Ok(response);
    }

    private string GenerateAccountNumber()
    {
        var random = new Random();

        return random.Next(100000000, 999999999).ToString();
    }
}