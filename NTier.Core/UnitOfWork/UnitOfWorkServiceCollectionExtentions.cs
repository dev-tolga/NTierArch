using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTier.Core.UnitOfWork
{
  public static  class UnitOfWorkServiceCollectionExtentions
    {
        public static List<IDbConnection> DbConnectionStringPool { get; set; } = new List<IDbConnection>();

        public static IServiceCollection AddUnitOfWork<TContext>(this IServiceCollection services, IConfiguration configuration)
            where TContext : DbContext
        {
            services.AddScoped<IUnitOfWork, UnitOfWork<TContext>>();
            services.AddScoped<IUnitOfWork<TContext>, UnitOfWork<TContext>>();
            services.AddScoped<IDbConnection, SqlConnection>((sp) =>
            {
                var connection = GetConnection<TContext>(sp, configuration);
                AddToDbConnectionPool(connection);
                return GetConnection<TContext>(sp, configuration);
            });
            return services;

        }

        private static void AddToDbConnectionPool(SqlConnection connection)
        {
            DbConnectionStringPool.Add(connection);
        }

        private static SqlConnection GetConnection<TContext>(IServiceProvider serviceProvider, IConfiguration configuration) where TContext : DbContext
        {
            var dbContext = serviceProvider.GetRequiredService<TContext>();
            var connectionString = configuration.GetSection(dbContext.GetType().Name + "Conn").Value;
            return new SqlConnection(connectionString);
        }
    }
}
