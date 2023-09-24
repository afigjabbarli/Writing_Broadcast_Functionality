using Microsoft.AspNetCore.Mvc;
using Pustok.Areas.Admin.ViewModels.AlertMessage;
using Pustok.Database;
using Pustok.Services.Abstracts;
using Pustok.ViewModels;

namespace Pustok.Areas.Admin.ViewComponents;

public class NavbarAlertMessagesViewComponent : ViewComponent
{
    private readonly PustokDbContext _pustokDbContext;
    private readonly IUserService _userService;

    public NavbarAlertMessagesViewComponent(
        PustokDbContext pustokDbContext,
        IUserService userService)
    {
        _pustokDbContext = pustokDbContext;
        _userService = userService;
    }

    public IViewComponentResult Invoke()
    {
        var alertMessages = _pustokDbContext.AlertMessages
            .Where(am => am.UserId == _userService.CurrentUser.Id)
            .OrderByDescending(o => o.CreatedAt)
            .Select(am => new AlertMessageViewModel
            {
                Id = am.Id,
                Title = am.Title,
                Content = am.Content,
                CreatedAt = am.CreatedAt
            })
            .ToList();
        
        return View(alertMessages);
    }
}
