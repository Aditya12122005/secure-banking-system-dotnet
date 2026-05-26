using Banking.Domain.Entities;

namespace Banking.Application.Interfaces;

public interface ITransactionRepository
{
    Task AddTransactionAsync(Transaction transaction);

    Task<List<Transaction>> GetTransactionsByAccountIdAsync(Guid accountId);

    Task SaveChangesAsync();

    Task<decimal> GetTodayTransferTotalAsync(Guid senderAccountId);
}