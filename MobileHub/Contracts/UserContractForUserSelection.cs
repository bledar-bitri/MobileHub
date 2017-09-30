using SecurityModel;

namespace Contracts
{
    public class UserContractForUserSelection
    {
        public int UserId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EMailAddress { get; set; }
        
        public UserContractForUserSelection()
        {
            
        }
        public UserContractForUserSelection(User user)
        {
            UserId = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            EMailAddress = user.EMailAddress;
        }

        public User ToEntity()
        {
            var user = new User
            {
                Id = UserId,
                FirstName = FirstName,
                LastName = LastName,
            };
            return user;
        }
    }
}
