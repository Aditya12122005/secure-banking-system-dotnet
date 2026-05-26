namespace Banking.Application.DTOs.Transaction;

public class TransferRequestDto
{
    public string SenderAccountNumber { get; set; } = string.Empty;

    public string ReceiverAccountNumber { get; set; } = string.Empty;

    public decimal Amount { get; set; }
}