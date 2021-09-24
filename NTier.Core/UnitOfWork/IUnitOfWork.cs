using Microsoft.EntityFrameworkCore;
using NTier.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTier.Core.UnitOfWork
{
   public interface IUnitOfWork :IDisposable
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, new();

        int SaveChanges();
        Task<int> SaveChangesAsync();

        void BeginTransaction();
        void CommitTransaction();
    }
    public interface IUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    {
    }

}
