using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTier.Core.Utils
{
   public class ContextContainer
    {
        private readonly List<Type> _contextTypes;
        private readonly IDictionary<Type, DbContext> _resolvedContextsByContextType;
        private readonly IDictionary<Type, DbContext> _resolvedContextsByEntityType;
        private readonly IDictionary<Type, IDictionary<Type, bool>> _contextEntityTypes;
        //TODO: Dışarıdan IDbConnection geçmek gerekmeyebilir aşağıda connection stringi konfigurasyondan okuyoruz artık.
        private readonly IDbConnection _connection;
        private readonly IConfiguration Configuration;

       

        public ContextContainer(IDbConnection connection, IConfiguration configuration)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _contextTypes = new List<Type>();
            _resolvedContextsByContextType = new Dictionary<Type, DbContext>();
            _resolvedContextsByEntityType = new Dictionary<Type, DbContext>();
            _contextEntityTypes = new Dictionary<Type, IDictionary<Type, bool>>();
            Configuration = configuration;
        }

        public void AddContext(Type contextType)
        {
            if (contextType != null)
            {
                _contextTypes.Add(contextType);
            }
        }

        public DbContext GetDbContextInstance(Type entityType)
        {
            if (_resolvedContextsByEntityType.ContainsKey(entityType))
            {
                return _resolvedContextsByEntityType[entityType];
            }

            // resolve DbContext by entity
            foreach (var contextType in _contextTypes)
            {
                if (DbSetDefinitionsContainsEntity(contextType, entityType))
                {
                    if (_resolvedContextsByContextType.ContainsKey(contextType))
                    {
                        _resolvedContextsByEntityType[entityType] = _resolvedContextsByContextType[contextType];
                        return _resolvedContextsByContextType[contextType];
                    }

                    DbContext dbContext = CreateDbContextInstance(contextType);

                    _resolvedContextsByContextType[contextType] = dbContext;
                    _resolvedContextsByEntityType[entityType] = dbContext;

                    return dbContext;
                }
            }

            throw new Exception("A suitable DbContext type cannot be found.");
        }

        private DbContext CreateDbContextInstance(Type contextType)
        {
            var optionsBuilderType = typeof(DbContextOptionsBuilder<>);
            var genericTypeArgs = new Type[] { contextType };
            var constructedOptionsType = optionsBuilderType.MakeGenericType(genericTypeArgs);
            dynamic optionsBuilder = Activator.CreateInstance(constructedOptionsType);
            DbContextOptions options = ConfigureOptions(optionsBuilder, contextType);
            return (DbContext)Activator.CreateInstance(contextType, options);
        }

        private DbContextOptions<T> ConfigureOptions<T>(DbContextOptionsBuilder<T> optionsBuilder, Type contextType) where T : DbContext
        {
            var connStringName = Configuration.GetSection(contextType.Name + "Conn").Value;

            optionsBuilder.UseSqlServer(connStringName);
            optionsBuilder.EnableSensitiveDataLogging(true);

            return optionsBuilder.Options;
        }

        private bool DbSetDefinitionsContainsEntity(Type contextType, Type entityType)
        {
            if (!_contextEntityTypes.ContainsKey(contextType))
            {
                _contextEntityTypes[contextType] = new Dictionary<Type, bool>();
            }
            if (_contextEntityTypes[contextType].ContainsKey(entityType))
            {
                return _contextEntityTypes[contextType][entityType];
            }
            var properties = TypeDescriptor.GetProperties(contextType);
            foreach (PropertyDescriptor property in properties)
            {
                if (property.PropertyType.IsGenericType && property.PropertyType.GenericTypeArguments.Length > 0)
                {
                    var typeDef = property.PropertyType.GetGenericTypeDefinition();
                    if (typeDef == typeof(DbSet<>))
                    {
                        var contains = property.PropertyType.GenericTypeArguments.Contains(entityType);
                        if (contains)
                        {
                            _contextEntityTypes[contextType][entityType] = true;
                            return true;
                        }
                    }
                }
            }
            _contextEntityTypes[contextType][entityType] = false;
            return false;
        }
        public IEnumerable<DbContext> Contexts
        {
            get
            {
                foreach (var context in _resolvedContextsByContextType)
                {
                    yield return context.Value;
                }
            }
        }
    }
}
