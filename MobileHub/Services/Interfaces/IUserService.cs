using System.Collections.Generic;
using Contracts;

namespace Services
{
    public interface IUserService
    {
        #region Data Access

        List<UserContract> AddUser(UserContract user, out string statistics);
        List<UserContract> AddUsers(List<UserContract> users, out string statistics);
        List<UserContract> GetAllUsers();
        List<UserContractForUserSelection> GetUsersForUserSelection();
        UserContract GetUser(int userId);

        #endregion

    }
}
