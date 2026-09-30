using ContactApp.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace ContactApp.Services
{
    public interface IUserService
    {
        Task<List<ApplicationUser>> GetUsersAsync();
        Task<ApplicationUser?> GetUserByIdAsync(string id);
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);

        Task<IdentityResult> UpdateUserAsync(ApplicationUser user);
        Task<IdentityResult> DeleteUserAsync(string id);
        Task<IList<string>> GetUserRolesAsync(ApplicationUser user);

        Task<IdentityResult> AddUserToRoles(ApplicationUser user, IEnumerable<string> roles);

        Task<IdentityResult> RemoveUserFromRoles(ApplicationUser user, IEnumerable<string> roles);
        Task<bool> UserExists(string userName);
    }
}
