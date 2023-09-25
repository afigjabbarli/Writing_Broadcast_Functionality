using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pustok.Database;
using Pustok.Services.Abstracts;
using Pustok.ViewModels;

namespace Pustok.ViewComponents
{
    
    public class NavbarOrderStatusNotificationsViewComponent : ViewComponent
    {
        private readonly IUserService _userService;
        private readonly PustokDbContext _dbContext;

        public NavbarOrderStatusNotificationsViewComponent(IUserService userService, PustokDbContext dbContext)
        {
            _userService = userService;
            _dbContext = dbContext;
        }

        public IViewComponentResult Invoke()
        {
            if(_userService.IsCurrentUserAuthenticated())
            {
               var OrderStatusNotifications = _dbContext.AlertMessages
                   .Where(am => am.UserId == _userService.CurrentUser.Id)
                   .Select(am => new OrderStatusNotificationViewModel
                   {
                       Title = am.Title,
                       Content = am.Content,
                       CreatedAt = am.CreatedAt,
                      
                   })
                   .ToList();
                
                return View(OrderStatusNotifications);
            }
            var EmptyOrderStatusNotifications = new List<OrderStatusNotificationViewModel>();
            return View(EmptyOrderStatusNotifications);  
        }
    }
}
