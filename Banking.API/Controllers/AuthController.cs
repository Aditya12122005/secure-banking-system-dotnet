using Banking.Application.DTOs.Auth;
using Banking.Application.Interfaces;
using Banking.Domain.Entities;

using Microsoft.AspNetCore.Mvc;

namespace Banking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    private readonly IJwtService _jwtService;

    private readonly IOtpService _otpService;

    private readonly IRefreshTokenService _refreshTokenService;

    public AuthController(
        IUserRepository userRepository,
        IJwtService jwtService,
        IOtpService otpService,
        IRefreshTokenService refreshTokenService)
    {
        _userRepository = userRepository;

        _jwtService = jwtService;

        _otpService = otpService;

        _refreshTokenService = refreshTokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDto request)
    {
        var existingUser = await _userRepository
            .GetByEmailAsync(request.Email);

        if (existingUser != null)
        {
            return BadRequest("User already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),

            FullName = request.FullName,

            Email = request.Email,

            PhoneNumber = request.PhoneNumber,

            PasswordHash = BCrypt.Net.BCrypt
                .HashPassword(request.Password)
        };

        await _userRepository.AddUserAsync(user);

        await _userRepository.SaveChangesAsync();

        return Ok("User registered successfully.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto request)
    {
        var user = await _userRepository
            .GetByEmailAsync(request.Email);

        if (user == null)
        {
            return Unauthorized("Invalid email or password.");
        }

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(
            request.Password,
            user.PasswordHash);

        if (!isPasswordValid)
        {
            return Unauthorized("Invalid email or password.");
        }

        var otp = _otpService.GenerateOtp(user.Email);

        return Ok(new
        {
            Message = "OTP generated successfully.",

            Otp = otp
        });
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp(
        VerifyOtpRequestDto request)
    {
        var user = await _userRepository
            .GetByEmailAsync(request.Email);

        if (user == null)
        {
            return Unauthorized("User not found.");
        }

        var isOtpValid = _otpService
            .VerifyOtp(request.Email, request.Otp);

        if (!isOtpValid)
        {
            return Unauthorized("Invalid or expired OTP.");
        }

        var token = _jwtService.GenerateToken(user);

        var refreshToken =
            _refreshTokenService.GenerateRefreshToken();

        await _refreshTokenService
            .SaveRefreshTokenAsync(
                user,
                refreshToken);

        var response = new AuthResponseDto
        {
            Token = token,

            RefreshToken = refreshToken,

            Email = user.Email,

            FullName = user.FullName
        };

        return Ok(response);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(
        RefreshTokenRequestDto request)
    {
        var storedToken =
            await _refreshTokenService
                .GetRefreshTokenAsync(
                    request.RefreshToken);

        if (storedToken == null)
        {
            return Unauthorized(
                "Invalid refresh token.");
        }

        if (storedToken.ExpiryDate < DateTime.UtcNow)
        {
            return Unauthorized(
                "Refresh token expired.");
        }

        var user = storedToken.User;

        var newAccessToken =
            _jwtService.GenerateToken(user);

        return Ok(new
        {
            Token = newAccessToken
        });
    }
}