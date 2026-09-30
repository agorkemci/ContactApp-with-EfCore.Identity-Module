using ContactApp.Models.Identity;
using ContactApp.Models.ViewModels;
using ContactApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContactApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public UsersController(IUserService userService, IRoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetUsersAsync();
            return View(users);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();  
        }
        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel userViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userViewModel);
            }

            // 1. Kullanıcı zaten var mı kontrolü (varsa işlemi durdur ve sayfaya dön)
            if (await _userService.UserExists(userViewModel.UserName!))
            {
                ModelState.AddModelError(string.Empty, "User already defined!");
                return View(userViewModel);
            }

            // 2. Kullanıcı nesnesini hazırla
            var user = new ApplicationUser()
            {
                UserName = userViewModel.UserName,
                Email = userViewModel.Email
            };

            // 3. Kullanıcıyı kaydet
            var result = await _userService.CreateUserAsync(user, userViewModel.Password!);

            // 4. Başarılıysa Index'e yönlendir
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            // 5. Identity/Servis hataları varsa modele bas ve View'a dön
            foreach (var err in result.Errors)
            {
                ModelState.AddModelError(string.Empty, err.Description);
            }

            return View(userViewModel);
        }
    }
}
