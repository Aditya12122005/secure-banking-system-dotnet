using Banking.Application.Interfaces;
using Banking.Domain.Entities;
using Banking.Persistence.Context;

using Microsoft.EntityFrameworkCore;

namespace Banking.Persistence.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly BankingDbContext _context;

    public AccountRepository(BankingDbContext context)
    {
        _context = context;
    }

    public async Task AddAccountAsync(Account account)
    {
        await _context.Accounts.AddAsync(account);
    }

    public async Task<List<Account>> GetAccountsByUserIdAsync(Guid userId)
    {
        return await _context.Accounts
            .Where(a => a.UserId == userId)
            .ToListAsync();
    }

    public async Task<Account?> GetByAccountNumberAsync(string accountNumber)
    {
        return await _context.Accounts
            .FirstOrDefaultAsync(a =>
                a.AccountNumber == accountNumber);
    }

    public async Task UpdateAccountAsync(Account account)
    {
        _context.Accounts.Update(account);

        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}