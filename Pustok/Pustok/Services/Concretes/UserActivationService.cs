using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Pustok.Database;
using Pustok.Database.Models;
using Pustok.Services.Abstracts;
using System;

namespace Pustok.Services.Concretes;

public class UserActivationService : IUserActivationService
{
    private readonly PustokDbContext _pustokDbContext;
    private readonly IEmailService _emailService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly LinkGenerator _linkGenerator;

    public UserActivationService(
        PustokDbContext pustokDbContext,
        IEmailService emailService,
        IHttpContextAccessor httpContextAccessor,
        LinkGenerator linkGenerator)
    {
        _pustokDbContext = pustokDbContext;
        _emailService = emailService;
        _httpContextAccessor = httpContextAccessor;
        _linkGenerator = linkGenerator;
    }

    public void CreateAndSendActivationToken(User user)
    {
        var activation = new UserActivation
        {
            Token = Guid.NewGuid(),
            User = user,
            ExpireDate = DateTime.UtcNow.AddHours(2),
        };

        var activationRoute = _linkGenerator.GetPathByRouteValues
            (_httpContextAccessor.HttpContext, "register-account-verification", new { activation.Token });

        _pustokDbContext.UserActivations.Add(activation);

        _emailService.SendEmail("Account activation", activationRoute, user.Email);
    }

  

}
