using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Pustok.Contracts;
using Pustok.Services.Abstracts;

namespace Pustok.Hubs
{
    
    [Authorize]
    public class OrderStatusNotificationHub : Hub
    {
        private readonly ILogger<OrderStatusNotificationHub> _logger;
        private readonly IOrderStatusNotificationService _orderStatusNotificationService;
        private readonly IUserService _userService;

        public OrderStatusNotificationHub(ILogger<OrderStatusNotificationHub> logger, IOrderStatusNotificationService orderStatusNotificationService, IUserService userService)
        {
            _logger = logger;
            _orderStatusNotificationService = orderStatusNotificationService;
            _userService = userService;
        }

        public override Task OnConnectedAsync()
        {
            _orderStatusNotificationService.AddConnectionId(_userService.CurrentUser, Context.ConnectionId);
            _logger.LogInformation($"New connection established : {Context.ConnectionId}");
            return base.OnConnectedAsync(); 
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _orderStatusNotificationService.RemoveConnectionId(_userService.CurrentUser, Context.ConnectionId);
            _logger.LogInformation($"Connection finished established : {Context.ConnectionId}");
            return base.OnDisconnectedAsync(exception);    
        }
    }
}
