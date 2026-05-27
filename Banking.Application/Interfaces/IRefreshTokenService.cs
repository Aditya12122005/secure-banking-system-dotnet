using Banking.Domain.Entities;

namespace Banking.Application.Interfaces;

public interface IRefreshTokenService
{
    string GenerateRefreshToken();

    Task SaveRefreshTokenAsync(
        User user,
        string refreshToken);

    Task<RefreshToken?> GetRefreshTokenAsync(
        string token);
}