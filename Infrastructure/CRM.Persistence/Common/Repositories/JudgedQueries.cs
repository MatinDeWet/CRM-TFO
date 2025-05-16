using CRM.Application.Common.Security.Contracts;
using CRM.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using RabbitLabsCRM.Application.Common.Repositories;

namespace CRM.Persistence.Common.Repositories;
public class JudgedQueries<TCtx> : ISecureQuery where TCtx : DbContext
{
    protected readonly TCtx _context;
    protected readonly IIdentityInfo _info;

    private readonly IEnumerable<IProtected> _protection;

    private readonly AccessRequirements _requirements;

    public JudgedQueries(TCtx context, IIdentityInfo info, AccessRequirements requirements, IEnumerable<IProtected> protection)
    {
        _context = context;
        _info = info;
        _protection = protection;

        _requirements = requirements;
    }

    public IQueryable<T> Secure<T>() where T : class
    {
        if (_info.HasRole(ApplicationRoleEnum.SuperAdmin))
        {
            return _context.Set<T>();
        }

        if (_protection.FirstOrDefault(x => x.IsMatch(typeof(T))) is IProtected<T> entityLock)
        {
            return entityLock.Secured(_info.GetIdentityId(), _requirements.GetRequirment());
        }

        return _context.Set<T>();
    }
}
