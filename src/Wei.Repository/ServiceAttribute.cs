using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public abstract class ServiceAttribute : Attribute
    {
        public ServiceAttribute(ServiceLifetime lifetime = ServiceLifetime.Scoped) => Lifetime = lifetime;


        public ServiceAttribute(Type serviceType, ServiceLifetime lifetime)
        {
            Lifetime = lifetime;
            ServiceType = serviceType;
        }

        public ServiceLifetime Lifetime { get; }

        public Type ServiceType { get; }

        public void Configure(IServiceCollection services, Type type)
        {
            var interfaceType = ServiceType;
            if (interfaceType == null)
            {
                var interfaces = type.GetInterfaces();
                interfaceType = interfaces.FirstOrDefault(x => x.Name == $"I{type.Name}") ?? interfaces.FirstOrDefault() ?? type;
            }

            services.TryAdd(new ServiceDescriptor(interfaceType, type, Lifetime));
        }
    }



    [AttributeUsage(AttributeTargets.Class)]
    public class SingletonServiceAttribute : ServiceAttribute
    {
        public SingletonServiceAttribute() : base(ServiceLifetime.Singleton)
        {

        }


        public SingletonServiceAttribute(Type serviceType) : base(serviceType, ServiceLifetime.Singleton)
        {


        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ScopedServiceAttribute : ServiceAttribute
    {
        public ScopedServiceAttribute() : base(ServiceLifetime.Scoped)
        {

        }


        public ScopedServiceAttribute(Type serviceType) : base(serviceType, ServiceLifetime.Scoped)
        {


        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class TransientServiceAttribute : ServiceAttribute
    {
        public TransientServiceAttribute() : base(ServiceLifetime.Transient)
        {

        }


        public TransientServiceAttribute(Type serviceType) : base(serviceType, ServiceLifetime.Transient)
        {


        }
    }
}
