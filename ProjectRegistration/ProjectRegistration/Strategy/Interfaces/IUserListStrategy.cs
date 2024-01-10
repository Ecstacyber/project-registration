using ProjectRegistration.Models;

namespace ProjectRegistration.Strategy.Interfaces
{
    public interface IUserListStrategy
    {
        Task<IEnumerable<User>> GetUserList(IDENTITYUSERContext context);
    }
}
