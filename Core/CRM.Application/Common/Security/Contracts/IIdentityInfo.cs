using CRM.Domain.Enums;

namespace CRM.Application.Common.Security.Contracts;

public interface IIdentityInfo
{
    int GetIdentityId();

    bool HasRole(ApplicationRoleEnum role);

    bool HasValue(string name);

    string GetValue(string name);
}
