using Microsoft.EntityFrameworkCore;
using ProjectRegistration.Models;
using ProjectRegistration.Strategy.Interfaces;

namespace ProjectRegistration.Strategy
{
    public class AllUserListStrategy : IUserListStrategy
    {
        public async Task<IEnumerable<User>> GetUserList(IDENTITYUSERContext context)
        {
            return await context.Users
                .Include(u => u.Department)
                .Where(x => x.Deleted == false)
                .ToListAsync();
        }
    }
}
