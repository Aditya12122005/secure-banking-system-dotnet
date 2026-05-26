using Banking.Persistence.Context;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

namespace Banking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly BankingDbContext _context;

    public AdminController(BankingDbContext context)
    {
        _context = context;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _context.Users
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> GetAllTransactions()
    {
        var transactions = await _context.Transactions
            .ToListAsync();

        return Ok(transactions);
    }

    [HttpGet("fraud-logs")]
    public async Task<IActionResult> GetFraudLogs()
    {
        var logs = await _context.FraudLogs
            .ToListAsync();

        return Ok(logs);
    }
}