using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhosTurnServer.ApplictionModels.Models;

namespace WhosTurnServer.Repository.IRepository
{
    public interface IUserRepository
    {
        bool IsUniqueUser(string email);
        User Authenticate(string email, string password);
        User Register(string email, string password);
        List<User> GetUsers();
    }
}
