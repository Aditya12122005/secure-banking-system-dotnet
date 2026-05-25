namespace Banking.Domain.Entities;

public class Transaction
{
    public Guid Id { get; set; }

    public Guid SenderAccountId { get; set; }

    public Guid ReceiverAccountId { get; set; }

    public decimal Amount { get; set; }

    public string TransactionType { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public Account SenderAccount { get; set; } = null!;

    public Account ReceiverAccount { get; set; } = null!;
}