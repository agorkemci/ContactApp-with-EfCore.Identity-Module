using ContactApp.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ContactApp.Services
{
    public class UserManagerService : IUserService
    {
        private readonly UserManager<ApplicationUser> _manager;

        public UserManagerService(UserManager<ApplicationUser> manager)
        {
            _manager = manager;
        }

        public async Task<IdentityResult> AddUserToRoles(ApplicationUser user, IEnumerable<string> roles)
        {
            return await _manager.AddToRolesAsync(user, roles);
        }

        public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password)
        {
            return await _manager.CreateAsync(user, password);
        }

        public async Task<IdentityResult> DeleteUserAsync(string id)
        {
            var user = await _manager.FindByIdAsync(id);
            if (user == null) {
                return IdentityResult.Failed(new IdentityError() { Description = "User not found." });
            }
            return await _manager.DeleteAsync(user);
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string id)
        {
            return await _manager.FindByIdAsync(id);  
        }

        public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
        {
            return await _manager.GetRolesAsync(user);         
        }

        public async Task<List<ApplicationUser>> GetUsersAsync()
        {
            return await _manager.Users.ToListAsync();
        }

        public async Task<IdentityResult> RemoveUserFromRoles(ApplicationUser user, IEnumerable<string> roles)
        {
            return await _manager.RemoveFromRolesAsync(user, roles);
        }

        public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user)
        {
            var existUser=await _manager.FindByIdAsync(user.Id);
            if (existUser==null)
            {
                return IdentityResult.Failed(new IdentityError() { Description = "User not found to update" });
            }
            existUser.UserName = user.UserName;
            existUser.Email = user.Email;
            return await _manager.UpdateAsync(existUser);
        }

        public async Task<bool> UserExists(string userName)
        {
            var user=await _manager.FindByNameAsync(userName);
            if(user==null)
                { return false; }
            return true;
        }
    }
}
