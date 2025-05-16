using System.Security.Claims;

namespace CRM.Application.Common.Security.Contracts;

public interface IInfoSetter : IList<Claim>
{
    void SetUser(IEnumerable<Claim> claims);
}
