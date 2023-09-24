using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Pustok.Database.Models;
using Pustok.Services.Abstracts;

namespace Pustok.Hubs;

[Authorize]
public class AlertMessageHub : Hub
{
    private readonly ILogger<AlertMessageHub> _logger;
    private readonly IAlertMessageService _alertMessageService;
    private readonly IUserService _userService;

    public AlertMessageHub(
        ILogger<AlertMessageHub> logger,
        IAlertMessageService alertMessageService,
        IUserService userService)
    {
        _logger = logger;
        _alertMessageService = alertMessageService;
        _userService = userService;
    }

    public override Task OnConnectedAsync()
    {
        _alertMessageService.AddConnectionId(_userService.CurrentUser, Context.ConnectionId);

        _logger.LogInformation($"New connection established : {Context.ConnectionId}");

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        _alertMessageService.RemoveConnectionId(_userService.CurrentUser, Context.ConnectionId);

        _logger.LogInformation($"Connection finished established : {Context.ConnectionId}");

        return base.OnDisconnectedAsync(exception);
    }

   
}
