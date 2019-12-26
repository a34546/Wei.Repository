using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Wei.Repository
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddRepository(this IServiceCollection services,
            Action<DbContextOptionsBuilder> options)
        {
            services.AddDbContext<BaseDbContext>(options);
            services.AddScoped<DbContext, BaseDbContext>();
            services.AddRepository();
            return services;
        }
        public static IServiceCollection AddRepository<TDbContext>(this IServiceCollection services,
            Action<DbContextOptionsBuilder> options) where TDbContext : BaseDbContext
        {
            services.AddDbContext<TDbContext>(options);
            services.AddScoped<DbContext, TDbContext>();
            services.AddRepository();
            return services;
        }

        private static IServiceCollection AddRepository(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            var assemblies = AppDomain.CurrentDomain.GetCurrentPathAssembly().Where(x => !(x.GetName().Name.Equals("Wei.Repository")));
            services.AddRepository(assemblies, typeof(IRepository<>));
            services.AddRepository(assemblies, typeof(IRepository<,>));
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient(typeof(IRepository<,>), typeof(Repository<,>));
            return services;
        }
        private static void AddRepository(this IServiceCollection services, IEnumerable<Assembly> assemblies, Type baseType)
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
                    var serviceDescriptor = new ServiceDescriptor(interfaceType, type, ServiceLifetime.Transient);
                    if (!services.Contains(serviceDescriptor)) services.Add(serviceDescriptor);
                }
            }
        }
    }
}
