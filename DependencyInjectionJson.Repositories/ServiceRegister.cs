using DependencyInjectionJson.XCutting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjectionJson.Repositories
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection RegisterDependencyInjectionRepositories(this IServiceCollection services, Dictionary<string, DependencyInjectionInfo> map)
        {
            var injectionData = ServiceRegisterTool.GetAssemblyInjetedDependencies("DependencyInjectionJson.Repositories", map);
            foreach(var injection in injectionData)
            {
                try
                {
                    var serviceType = Type.GetType(injection.ServiceType);
                    var inplementationType = Type.GetType(injection.ImplementationType);

                    if (serviceType != null && inplementationType != null && inplementationType.GetInterfaces().Any(c => c == serviceType))
                    {
                        var lifetime = injection.Lifetime;
                        services.Add(new ServiceDescriptor(serviceType: serviceType,
                                               implementationType: inplementationType,
                                               lifetime: lifetime));
                    }
                }
                catch (Exception ex)
                {
                    //Posiblemente no se encuentra algun tipo
                }
            }
            return services;
        }
    }
}
