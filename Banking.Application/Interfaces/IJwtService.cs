using Banking.Domain.Entities;

namespace Banking.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}