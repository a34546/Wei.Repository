using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Wei.Repository
{
    public static class ServiceCollectionExtension
    {

        public static IServiceCollection AddRepository<TDbContext>(this IServiceCollection services,
            Action<DbContextOptionsBuilder> options, ServiceLifetime repositoryLifetime = ServiceLifetime.Scoped) where TDbContext : BaseDbContext
        {
            services.AddDbContext<TDbContext>(options);
            services.AddScoped<DbContext, TDbContext>();
            services.AddRepository(repositoryLifetime);
            return services;
        }

        private static IServiceCollection AddRepository(this IServiceCollection services, ServiceLifetime serviceLifetime)
        {
            services.TryAddScoped<DbContextFactory>();
            services.TryAddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.TryAddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            var assemblies = AppDomain.CurrentDomain.GetCurrentPathAssembly().Where(x => !(x.GetName().Name.Equals("Wei.Repository")));
            services.AddRepository(assemblies, typeof(IRepository<>), serviceLifetime);
            services.AddRepository(assemblies, typeof(IRepository<,>), serviceLifetime);
            services.TryAdd(new ServiceDescriptor(typeof(IRepository<>), typeof(Repository<>), serviceLifetime));
            services.TryAdd(new ServiceDescriptor(typeof(IRepository<,>), typeof(Repository<,>), serviceLifetime));
            return services;
        }
        private static void AddRepository(this IServiceCollection services, IEnumerable<Assembly> assemblies, Type baseType, ServiceLifetime serviceLifetime)
        {

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes()
                                    .Where(x => x.IsClass
                                            && !x.IsAbstract
                                            && x.BaseType != null
                                            && x.HasImplementedRawGeneric(baseType));
                foreach (var type in types)
                {
                    var interfaces = type.GetInterfaces();
                    var interfaceType = interfaces.FirstOrDefault(x => x.Name == $"I{type.Name}");
                    if (interfaceType == null) interfaceType = type;
                    var serviceDescriptor = new ServiceDescriptor(interfaceType, type, serviceLifetime);
                    services.TryAdd(serviceDescriptor);
                }
            }

        }
    }
}
