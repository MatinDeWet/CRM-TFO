﻿using CRM.Domain.Common.ResultTypes;

namespace CRM.Domain.Common.Messaging;
public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken);
}
