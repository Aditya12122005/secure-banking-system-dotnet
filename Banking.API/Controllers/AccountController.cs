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

    private readonly ICacheService _cacheService;

    public AccountController(
        IAccountRepository accountRepository,
        ICacheService cacheService)
    {
        _accountRepository = accountRepository;

        _cacheService = cacheService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateAccount(
        CreateAccountRequestDto request)
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var account = new Account
        {
            Id = Guid.NewGuid(),

            AccountNumber = GenerateAccountNumber(),

            Balance = 0,

            AccountType = request.AccountType,

            IsActive = true,

            IsFrozen = false,

            CreatedAt = DateTime.UtcNow,

            UserId = Guid.Parse(userId)
        };

        await _accountRepository
            .AddAccountAsync(account);

        await _accountRepository
            .SaveChangesAsync();

        // Remove Old Cache
        await _cacheService.RemoveAsync("accounts");

        return Ok(new
        {
            Message = "Account created successfully.",

            AccountNumber = account.AccountNumber
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetAccounts()
    {
        var cacheKey = "accounts";

        // Check Redis Cache
        var cachedAccounts =
            await _cacheService
                .GetAsync<List<Account>>(cacheKey);

        if (cachedAccounts != null)
        {
            return Ok(new
            {
                Source = "Redis Cache",

                Data = cachedAccounts
            });
        }

        // Fetch From Database
        var accounts =
            await _accountRepository
                .GetAllAccountsAsync();

        // Store In Redis
        await _cacheService.SetAsync(
            cacheKey,
            accounts,
            TimeSpan.FromMinutes(5));

        return Ok(new
        {
            Source = "PostgreSQL Database",

            Data = accounts
        });
    }

    private static string GenerateAccountNumber()
    {
        var random = new Random();

        return random.Next(
            100000000,
            999999999).ToString();
    }
}