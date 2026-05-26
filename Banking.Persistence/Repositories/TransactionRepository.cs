using Banking.Application.Interfaces;
using Banking.Domain.Entities;
using Banking.Persistence.Context;

using Microsoft.EntityFrameworkCore;

namespace Banking.Persistence.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly BankingDbContext _context;

    public TransactionRepository(BankingDbContext context)
    {
        _context = context;
    }

    public async Task AddTransactionAsync(Transaction transaction)
    {
        await _context.Transactions.AddAsync(transaction);
    }

    public async Task<List<Transaction>> GetTransactionsByAccountIdAsync(Guid accountId)
    {
        return await _context.Transactions
    .Include(t => t.SenderAccount)
    .Include(t => t.ReceiverAccount)
            .Where(t =>
                t.SenderAccountId == accountId ||
                t.ReceiverAccountId == accountId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}