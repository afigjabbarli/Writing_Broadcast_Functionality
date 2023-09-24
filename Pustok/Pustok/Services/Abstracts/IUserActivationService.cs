using Pustok.Database.Models;

namespace Pustok.Services.Abstracts;

public interface IUserActivationService
{
    void CreateAndSendActivationToken(User user);
}
