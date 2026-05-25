using Banking.Domain.Entities;

namespace Banking.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);

    Task AddUserAsync(User user);

    Task SaveChangesAsync();
}