namespace Banking.Application.DTOs.Transaction;

public class TransactionHistoryDto
{
    public string SenderAccountNumber { get; set; } = string.Empty;

    public string ReceiverAccountNumber { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string TransactionType { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime TransactionDate { get; set; }
}