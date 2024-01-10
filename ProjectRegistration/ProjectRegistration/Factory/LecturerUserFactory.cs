using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectRegistration.Factory.Interfaces;
using ProjectRegistration.Models;

namespace ProjectRegistration.Factory
{
    public class LecturerUserFactory : IUserFactory
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IDENTITYUSERContext _context;
        private readonly IUserEmailStore<User> _emailStore;


        public LecturerUserFactory(UserManager<User> userManager,
            IUserStore<User> userStore,
            IDENTITYUSERContext context)
        {
            _userManager = userManager;
            _userStore = userStore;
            _context = context;
            _emailStore = GetEmailStore();
        }

        public async Task<User?> CreateUser(User user)
        {
            var newUser = Activator.CreateInstance<User>();
            newUser.UserId = user.UserId;
            await _userStore.SetUserNameAsync(newUser, newUser.UserId, CancellationToken.None);
            await _emailStore.SetEmailAsync(newUser, newUser.UserId + "@gm.uit.edu.vn", CancellationToken.None);
            var result = await _userManager.CreateAsync(newUser, user.UserId);

            if (result.Succeeded)
            {
                newUser.CreatedDateTime = DateTime.Now;
                newUser.Fullname = user.Fullname;
                newUser.PhoneNumber = user.PhoneNumber;
                newUser.Gender = user.Gender;
                newUser.DateOfBirth = user.DateOfBirth;
                newUser.DepartmentId = user.DepartmentId;
                newUser.UserTypeId = user.UserTypeId;

                await _userManager.AddToRoleAsync(newUser, "Lecturer");
                _context.Update(newUser);
                await _context.SaveChangesAsync();
                return newUser;
            }
            return null;
        }

        private IUserEmailStore<User> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<User>)_userStore;
        }
    }
}
