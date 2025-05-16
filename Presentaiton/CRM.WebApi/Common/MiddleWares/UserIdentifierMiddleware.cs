using System.Security.Claims;
using CRM.Application.Common.Security.Contracts;

namespace CRM.WebApi.Common.MiddleWares;

public class UserIdentifierMiddleware
{
    private readonly RequestDelegate _next;

    public UserIdentifierMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext context, IInfoSetter infoSetter)
    {
        if (context.User.Identity != null && context.User.Identity.IsAuthenticated)
        {
            IEnumerable<Claim> claims = context.User.Claims;

            infoSetter.SetUser(claims);
        }

        return _next(context);
    }
}
