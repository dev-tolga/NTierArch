using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NTier.Core.Repository;
using NTier.Core.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTier.Core.UnitOfWork
{
    public abstract class UnitOfWork : IUnitOfWork
    {
        private Dictionary<string, object> _repositories;
        protected ContextContainer _contextContainer;
        private static bool IsTransactionOpened;


        public IDbConnection Connection { get; set; }
        public IDbTransaction Transaction { get; set; }
        public IConfiguration Configuration { get; }

        protected UnitOfWork(IDbConnection dbConnection, IConfiguration configuration)
        {
            Connection = dbConnection;
            Configuration = configuration;
            _contextContainer = new ContextContainer(dbConnection, Configuration);
        }
        public void BeginTransaction()
        {
            if (Connection != null && Connection.State != ConnectionState.Open)
                Connection.Open();

            if (Transaction == null)
            {
                Transaction = Connection.BeginTransaction();
                IsTransactionOpened = true;
            }
        }

        public void CommitTransaction()
        {
            try
            {
                if (Transaction != null)
                {
                    SaveChanges();
                    Transaction.Commit();
                }
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message);
            }finally
            {
                IsTransactionOpened = false;
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();

            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            foreach (var context in _contextContainer.Contexts)
            {
                context.Dispose();
            }
            if (Connection.State == ConnectionState.Open)
                Connection.Close();
        }

        public IRepository<TEntity> GetRepository<TEntity>()
             where TEntity : class, new()
        {
            return GetRepositoryDetail<TEntity, EFRepository<TEntity>>();
        }
        private IRepository<TEntity> GetRepositoryDetail<TEntity, TRepo>()
            where TEntity : class, new()
            where TRepo : IRepository<TEntity>
        {
            var entityType = typeof(TEntity);
            var repoType = typeof(TRepo);
            var key = $"{entityType}::{repoType}::{IsTransactionOpened}";
            if (_repositories == null)
                _repositories = new Dictionary<string, object>();

            if (Connection != null && Connection.State != ConnectionState.Open)
                Connection.Open();

            var dbContext = _contextContainer.GetDbContextInstance(entityType);
            var dbName = Configuration.GetSection(dbContext.GetType().Name).Value;

            //if (Connection.Database != dbName)
            //{
            //    Connection.ChangeDatabase(dbName);
            //}

            if (!_repositories.ContainsKey(key))
            {
                if (repoType.Name.Contains(nameof(EFRepository<TEntity>)))
                {
                    if (IsTransactionOpened && dbContext.Database.CurrentTransaction == null)
                    {
                        dbContext.Database.UseTransaction((DbTransaction)Transaction);
                    }
                    _repositories[key] = new EFRepository<TEntity>(dbContext);
                }
                
            }

            return (IRepository<TEntity>)_repositories[key];
        }

        public int SaveChanges()
        {
            int result = 0;

            foreach (var context in _contextContainer.Contexts)
            {
                //Connection.ChangeDatabase(Configuration.GetSection(context.GetType().Name).Value);
                result += context.SaveChanges();
            }

            return result;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await Task.Run(() => SaveChanges());
        }

        
    }
    public class UnitOfWork<TContext> : UnitOfWork, IUnitOfWork<TContext> where TContext : DbContext
    {
        public UnitOfWork(IDbConnection connection, IConfiguration configuration) : base(connection, configuration)
        {
            _contextContainer.AddContext(typeof(TContext));
        }
    }
}
