using ContactApp.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ContactApp.Services
{
    public class RoleManagerService : IRoleService
    {

        private readonly RoleManager<ApplicationRole> roleManager;

        public RoleManagerService(RoleManager<ApplicationRole> roleManager)
        {
            this.roleManager = roleManager;
        }

        public async Task<IdentityResult> CreateRoleAsync(string roleName)
        {
            var role=new ApplicationRole() { Name = roleName };
            return await roleManager.CreateAsync(role);
        }

        public async Task<IdentityResult> DeleteRoleAsync(string Id)
        {
            var role = await roleManager.FindByIdAsync(Id);
            if(role is null)
            {
                return IdentityResult.Failed(new IdentityError() { Description="Role is not founded."});

            }
            return await roleManager.DeleteAsync(role);
        }

        public async Task<ApplicationRole> GetRoleByIdAsync(string Id)
        {
            return await roleManager.FindByIdAsync(Id);
        }

        public async Task<List<ApplicationRole>> GetRolesAsync()
        {
            return await roleManager.Roles.ToListAsync();   
        }

        public async Task<bool> RoleExists(string roleName)
        {
            var role=await roleManager.FindByNameAsync(roleName);
            if (role is not null)
                return true;
            return false;       
        }

        public async Task<IdentityResult> UpdateRoleAsync(ApplicationRole role)
        {
            var existing=await roleManager.RoleExistsAsync(role.Name);
            if (existing)
            {
                var existRole=await roleManager.FindByIdAsync(role.Id);
                existRole = role;
                return await roleManager.UpdateAsync(existRole);
            }
            return IdentityResult.Failed(new IdentityError() { Description="The updating process is failed."});
            

        }
    }
}
