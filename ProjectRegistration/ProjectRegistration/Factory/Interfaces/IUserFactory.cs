using ProjectRegistration.Models;

namespace ProjectRegistration.Factory.Interfaces
{
    public interface IUserFactory
    {
        Task<User?> CreateUser(User user);
    }

}
