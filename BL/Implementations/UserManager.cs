using BL.Interfaces;
using DAL.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BL.Implementations;

public class UserManager(IUserRepository repository) : Manager<IdentityUser>(repository), IUserManager 
{
    public IdentityUser GetUserById(string id)
    {
        return repository.GetUserById(id);
    }

    public bool CheckPassword(string loggedInUser, string user, string password)
    {
        return repository.CheckPassword(loggedInUser, user, password);
    }
}