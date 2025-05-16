
using System.Security.Claims;
using CRM.Application.Common.Security.Contracts;

namespace CRM.WebApi.Common.Filters;

public class UserIdentifierFilter(IInfoSetter infoSetter) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (
            context.HttpContext != null
            && context.HttpContext.User.Identity != null
            && context.HttpContext.User.Identity.IsAuthenticated)
        {
            IEnumerable<Claim> claims = context.HttpContext.User.Claims;

            infoSetter.SetUser(claims);
        }

        return await next(context);
    }
}
