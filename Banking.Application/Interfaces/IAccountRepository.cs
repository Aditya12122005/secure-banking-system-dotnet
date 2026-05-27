using Banking.Domain.Entities;

namespace Banking.Application.Interfaces;

public interface IAccountRepository
{
    Task AddAccountAsync(Account account);

    Task<List<Account>> GetAccountsByUserIdAsync(Guid userId);

    Task<Account?> GetByAccountNumberAsync(string accountNumber);

    Task<List<Account>> GetAllAccountsAsync();

    Task UpdateAccountAsync(Account account);

    Task SaveChangesAsync();
}