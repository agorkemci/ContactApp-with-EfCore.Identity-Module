using ContactApp.Models.Identity;
using ContactApp.Models.ViewModels;
using ContactApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContactApp.Controllers
{
    [Authorize(Roles ="Admin")]
    public class RolesController : Controller
    {
        private readonly IRoleService roleService;

        public RolesController(IRoleService roleService)
        {
            this.roleService = roleService;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await roleService.GetRolesAsync();
            return View(roles);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Name")] RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                if (await roleService.RoleExists(roleViewModel.Name))
                {
                    ModelState.AddModelError(string.Empty, "The role is already exists!");
                    return View(roleViewModel);
                }
                var result = await roleService.CreateRoleAsync(roleViewModel.Name);//özünde string gönderiyoruz ancak bu ApplicationRole olarak return ediliyor.
                if (result.Succeeded)
                    return RedirectToAction("Index");
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                }

            }
            return View(roleViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var role = await roleService.GetRoleByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            var roleViewModel = new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name
            };
            return View(roleViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name")] RoleViewModel roleViewModel)
        {
            if (id != roleViewModel.Id) { return NotFound(); }

            if (ModelState.IsValid)
            {
                var role = new ApplicationRole { Id = roleViewModel.Id, Name = roleViewModel.Name };
                var result = await roleService.UpdateRoleAsync(role);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                }
            }
            return View(roleViewModel);
        }
        [HttpGet, ActionName("Delete")]
        public async Task<IActionResult> DeleteGet(string id)
        {
            if (id == null)
                return NotFound();
            var role = await roleService.GetRoleByIdAsync(id);
            if (role == null) { return NotFound(); }
            var roleViewModel = new RoleViewModel { Id = role.Id, Name = role.Name };
            return View(roleViewModel);
        }


        [HttpPost,ActionName("Delete")]
        public async Task<IActionResult> DeletePost(string id)
        {
            if (String.IsNullOrEmpty(id))
                return NotFound();
            var role = await roleService.GetRoleByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            var result = await roleService.DeleteRoleAsync(id);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return RedirectToAction("Index");

        }
    }
}
