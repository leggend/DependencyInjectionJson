using DependencyInjectionJson.XCutting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjectionJson.Services
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection RegisterDependencyInjectionServices(this IServiceCollection services, Dictionary<string, DependencyInjectionInfo> map)
        {
            var injectionData = ServiceRegisterTool.GetAssemblyIncetedDependencies("DependencyInjectionJson.Services", map);
            foreach (var injection in injectionData)
            {
                try
                {
                    var serviceType = Type.GetType(injection.ServiceType);
                    var inplementationType = Type.GetType(injection.ImplementationType);
                    var lifetime = injection.Lifetime;
                    services.Add(new ServiceDescriptor(serviceType: serviceType,
                                           implementationType: inplementationType,
                                           lifetime: lifetime));
                }catch(Exception ex)
                {
                    //Posiblemente no se encuentra algun tipo
                }
            }
            return services;
        }
    }
}
