using Pustok.Database.Models;
using Pustok.Services.Abstracts;

namespace Pustok.Services.Concretes;

public class AlertMessageService : IAlertMessageService
{
    private Dictionary<int, List<string>> _userConnectionIds;

    public AlertMessageService()
    {
        _userConnectionIds = new Dictionary<int, List<string>>();
    }

    public void AddConnectionId(User user, string connectionId)
    {
        if (_userConnectionIds.ContainsKey(user.Id))
        {
            List<string> connectionIds = _userConnectionIds[user.Id];
            connectionIds.Add(connectionId);
        }
        else
        {
            _userConnectionIds.Add(user.Id, new List<string> { connectionId });
        }
    }

    public void RemoveConnectionId(User user, string connectionId)
    {
        List<string> connectionIds = _userConnectionIds[user.Id];
        connectionIds.Remove(connectionId);

        if (!connectionIds.Any())
        {
            _userConnectionIds.Remove(user.Id);
        }
    }

    public List<string> GetConnectionIds(User user)
    {
        if (_userConnectionIds.ContainsKey(user.Id))
        {
            return _userConnectionIds[user.Id];
        }

        return new List<string>();
    }

   
}
