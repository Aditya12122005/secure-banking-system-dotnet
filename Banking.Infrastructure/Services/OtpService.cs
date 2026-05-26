using Banking.Application.Interfaces;

namespace Banking.Infrastructure.Services;

public class OtpService : IOtpService
{
    private static readonly Dictionary<string, (string Otp, DateTime Expiry)>
        _otpStore = new();

    public string GenerateOtp(string email)
    {
        var random = new Random();

        var otp = random.Next(100000, 999999).ToString();

        _otpStore[email] = (
            otp,
            DateTime.UtcNow.AddMinutes(5));

        return otp;
    }

    public bool VerifyOtp(string email, string otp)
    {
        if (!_otpStore.ContainsKey(email))
        {
            return false;
        }

        var storedOtp = _otpStore[email];

        if (storedOtp.Expiry < DateTime.UtcNow)
        {
            _otpStore.Remove(email);

            return false;
        }

        var isValid = storedOtp.Otp == otp;

        if (isValid)
        {
            _otpStore.Remove(email);
        }

        return isValid;
    }
}