using Pustok.Database.Models;

namespace Pustok.Services.Abstracts
{
    public interface IOrderStatusNotificationService
    {

        void AddConnectionId(User user, string connectionId);
        void RemoveConnectionId(User user, string connectionId);
        List<string> GetConnectionIds(User user);

    }
}
