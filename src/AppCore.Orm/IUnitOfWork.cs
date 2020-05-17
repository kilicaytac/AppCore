﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace AppCore.Orm
{
    public interface IUnitOfWork:IDisposable
    {
        Task BeginAsync(CancellationToken cancellationToken = default);
        Task CommitAsync(CancellationToken cancellationToken = default);
        Task RollbackAsync(CancellationToken cancellationToken = default);
    }
}