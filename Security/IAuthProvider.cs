using Domain.Context;
using Domain.Entities;
using Services;

namespace Security
{
    public interface IAuthProvider : IService<AuthContext>
    {
        string Encrypt(string value = "");
        bool Validate(string userEmail, string HashCode, Hashs type);
        User Authenticate(string email, string password);
        User GetCredentials(string email);
        User UpdateCredentials(User user, string password);
        Hash ProvideLogin(User user, string password);
        Hash ProvideLinkToRecovery(User user);
        string GenerateUserWithPassword(User user);
    }
}