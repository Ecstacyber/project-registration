using Microsoft.EntityFrameworkCore;
using ProjectRegistration.Models;
using ProjectRegistration.Strategy.Interfaces;

namespace ProjectRegistration.Strategy
{
    public class StudentListStrategy : IUserListStrategy
    {
        public async Task<IEnumerable<User>> GetUserList(IDENTITYUSERContext context)
        {
            return await context.Users
                .Where(u => u.UserTypeId == 100)
                .Include(u => u.Department)
                .Where(x => x.Deleted == false)
                .ToListAsync();
        }
    }
}
