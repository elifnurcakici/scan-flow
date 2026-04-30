using backend.Services.Interfaces;
namespace backend.Services;
public class FakePasswordService : IPasswordService
{
    public string HashPassword(string password) => password;

    public bool VerifyPassword(string password, string hash)
        => password == hash;
}