using ContactApp.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace ContactApp.Services
{
    public interface IRoleService
    {
        public Task<List<ApplicationRole>> GetRolesAsync();
        public Task<ApplicationRole> GetRoleByIdAsync(string Id);
        public Task<IdentityResult> CreateRoleAsync(string roleName);
        public Task<IdentityResult> UpdateRoleAsync(ApplicationRole role);
        public Task<IdentityResult> DeleteRoleAsync(string Id);
        public Task<bool> RoleExists(string roleName);



    }
}
