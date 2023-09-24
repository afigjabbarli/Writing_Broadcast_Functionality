using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Pustok.Services.Abstracts;
using Pustok.Services.Concretes;

namespace Pustok.Hubs;


[Authorize]
public class OnlineUserHub : Hub
{
    private readonly ILogger<OnlineUserHub> _logger;
    private readonly OnlineUserTracker _onlineUserTracker;
    private readonly IUserService _userService;
    private readonly IHubContext<StaffUsersViewHub> _hubContext;

    public OnlineUserHub(
        ILogger<OnlineUserHub> logger,
        OnlineUserTracker onlineUserTracker,
        IUserService userService,
        IHubContext<StaffUsersViewHub> hubContext)
    {
        _logger = logger;
        _onlineUserTracker = onlineUserTracker;
        _userService = userService;
        _hubContext = hubContext;
    }

    public override Task OnConnectedAsync()
    {
        if (!_onlineUserTracker.DoesUserHaveConnectionId(_userService.CurrentUser))
        {
            //Kimse eger users sehifesine baxirsa auto online statusun update olunmasi
            _hubContext.Clients
                .Group("users-view")
                .SendAsync("UserOnlineStatusUpdate",
                    new
                    {
                        UserId = _userService.CurrentUser.Id,
                        IsOnline = true,
                    });
        }

        //Cari veziyyet online olanlari bilmeyimiz ucunm
        _onlineUserTracker.AddConnectionId(_userService.CurrentUser, Context.ConnectionId);


        _logger.LogInformation($"New connection established : {Context.ConnectionId}");

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        _onlineUserTracker.RemoveConnectionId(_userService.CurrentUser, Context.ConnectionId);

        if (!_onlineUserTracker.DoesUserHaveConnectionId(_userService.CurrentUser))
        {
            _hubContext.Clients
                .Group("users-view")
                .SendAsync("UserOnlineStatusUpdate",
                    new
                    {
                        UserId = _userService.CurrentUser.Id,
                        IsOnline = false,
                    });
        }

        _logger.LogInformation($"Connection finished established : {Context.ConnectionId}");

        return base.OnDisconnectedAsync(exception);
    }
}
