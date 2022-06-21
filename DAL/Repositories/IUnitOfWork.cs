﻿using Microsoft.EntityFrameworkCore.Storage;
using DAL.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepository<Order> OrderRepository { get; }
        IBaseRepository<OrderDetail> OrderDetailRepository { get; }
        IBaseRepository<Product> ProductRepository { get; }

        void Save();
        Task SaveAsync(CancellationToken cancellationToken = default(CancellationToken));
        IDbContextTransaction StartNewTransaction();
        Task<IDbContextTransaction> StartNewTransactionAsync();
        Task<int> ExecuteSqlCommandAsync(string sql, object[] parameters, CancellationToken cancellationToken = default(CancellationToken));
    }
}
