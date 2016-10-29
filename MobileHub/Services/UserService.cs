using System;
using System.Collections.Generic;
using System.Linq;
using Contracts;
using DataAccessLayer.Managers.Security;
using SecurityModel;

namespace Services
{
    public class UserService : IUserService, IDisposable
    {
        private readonly UserDataManager _manager = new UserDataManager();

        public void Dispose()
        {
            _manager.Dispose();
        }

        #region Data Access

        public List<UserContract> AddUser(UserContract user, out string statistics)
        {
            var newUsers = _manager.SaveUsers(new List<User> {user.ToEntity()}, out statistics);
            return newUsers.Select(u => new UserContract(u)).ToList();
        }

        public List<UserContract> AddUsers(List<UserContract> users, out string statistics)
        {
            var newUsers = _manager.SaveUsers(users.Select(user => user.ToEntity()).ToList(), out statistics);
            return newUsers.Select(u => new UserContract(u)).ToList();
        }

        public List<UserContract> GetAllUsers()
        {
            var users = _manager.GetUsers();
            return users.Select(user => new UserContract(user)).ToList();

        }

        public UserContract GetUser(int userId)
        {
            var user = _manager.GetUser(userId);
            return new UserContract(user);
        }

        #endregion


    }
}
