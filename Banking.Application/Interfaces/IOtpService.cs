namespace Banking.Application.Interfaces;

public interface IOtpService
{
    string GenerateOtp(string email);

    bool VerifyOtp(string email, string otp);
}