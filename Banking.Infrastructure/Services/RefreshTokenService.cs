using Banking.Application.Interfaces;
using Banking.Domain.Entities;
using Banking.Persistence.Context;

using Microsoft.EntityFrameworkCore;

namespace Banking.Infrastructure.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly BankingDbContext _context;

    public RefreshTokenService(
        BankingDbContext context)
    {
        _context = context;
    }

    public string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString()
            + Guid.NewGuid().ToString();
    }

    public async Task SaveRefreshTokenAsync(
        User user,
        string refreshToken)
    {
        var token = new RefreshToken
        {
            Id = Guid.NewGuid(),

            Token = refreshToken,

            UserId = user.Id,

            ExpiryDate = DateTime.UtcNow.AddDays(7),

            IsRevoked = false
        };

        await _context.RefreshTokens
            .AddAsync(token);

        await _context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(
        string token)
    {
        return await _context.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r =>
                r.Token == token &&
                !r.IsRevoked);
    }
}