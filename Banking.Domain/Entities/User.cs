using System.Security.Principal;

namespace Banking.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public bool IsVerified { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Property
    public ICollection<Account> Accounts { get; set; } = new List<Account>();

    public string Role { get; set; } = "Customer";
}