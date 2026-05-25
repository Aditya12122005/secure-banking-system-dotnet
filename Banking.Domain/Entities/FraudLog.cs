namespace Banking.Domain.Entities;

public class FraudLog
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string RiskLevel { get; set; } = string.Empty;

    public string Reason { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}