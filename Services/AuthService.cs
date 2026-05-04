using AuthApi.Data;
using AuthApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Services;

public class AuthService
{
    private readonly AppDbContext _db;
    private readonly JwtService _jwt;

    public AuthService(AppDbContext db, JwtService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    public async Task<(bool Success, string Message)> RegisterAsync(
        string username, string password, string confirmPassword)
    {
        if (password != confirmPassword)
            return (false, "Password and Confirm Password do not match");

        var exists = await _db.Users.AnyAsync(u => u.Username == username);
        if (exists)
            return (false, "This username is already taken");

        var user = new User
        {
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return (true, "Registration successful");
    }

    public async Task<(bool Success, string Token, string Username)> LoginAsync(
        string username, string password)
    {
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == username);
        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return (false, string.Empty, string.Empty);

        var token = _jwt.GenerateToken(user);
        return (true, token, user.Username);
    }
}
