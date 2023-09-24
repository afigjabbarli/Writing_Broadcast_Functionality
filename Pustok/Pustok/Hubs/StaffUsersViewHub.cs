using Microsoft.AspNetCore.SignalR;

namespace Pustok.Hubs
{
    public class StaffUsersViewHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            Groups.AddToGroupAsync(Context.ConnectionId, "users-view");

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Groups.RemoveFromGroupAsync(Context.ConnectionId, "users-view");

            return base.OnDisconnectedAsync(exception);
        }

    }
}
