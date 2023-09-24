namespace Pustok.Areas.Admin.ViewModels.User;

public class UserViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public bool IsConfirmed { get; set; }
    public string Role { get; set; }
    public bool IsOnline { get; set; }
}
