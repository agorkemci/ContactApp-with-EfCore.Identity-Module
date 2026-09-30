namespace ContactApp.Models.ViewModels
{
    public class UserRoleViewModel
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public List<RoleViewModel>? Roles { get; set; }
        public string? Password { get; set; }
    }
}
