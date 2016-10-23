using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using DatabaseContext.Entities;

namespace Services
{
    public interface IUserService
    {
        #region Data Access

        List<UserContract> AddUser(UserContract user, out string statistics);
        List<UserContract> AddUsers(List<UserContract> users, out string statistics);
        List<UserContract> GetAllUsers();
        UserContract GetUser(int userId);

        #endregion

    }
}
