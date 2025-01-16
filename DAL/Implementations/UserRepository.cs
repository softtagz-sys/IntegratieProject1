using DAL.EF;
using DAL.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace DAL.Implementations;

public class UserRepository(PhygitalDbContext context) : Repository(context), IUserRepository
{
    public IdentityUser GetUserById(string id)
    {
        return context.Users.Find(id);
    }

    public bool CheckPassword(string loggedInUser, string user, string password)
    {
        if (loggedInUser != user)
        {
            return false;
        }
        
        var identityUser = context.Users.FirstOrDefault(u => u.UserName == user);
        if (identityUser == null)
        {
            return false;
        }

        var passwordHasher = new PasswordHasher<IdentityUser>();
        if (identityUser.PasswordHash != null)
        {
            var result = passwordHasher.VerifyHashedPassword(identityUser, identityUser.PasswordHash, password);
    
            return result == PasswordVerificationResult.Success;
        }
        return false;
    }
}