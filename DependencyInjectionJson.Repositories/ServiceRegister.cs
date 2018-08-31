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
            var assemblyName = "DependencyInjectionJson.Repositories";
            var assembly = System.Reflection.Assembly.Load(assemblyName);
            Microsoft.Extensions.DependencyModel.DependencyContextLoader.Default.Load(assembly);

            var injectionData = ServiceRegisterTool.GetAssemblyInjetedDependencies(assemblyName, map);
            foreach (var injection in injectionData)
            {
                try
                {
                    var interfaceType = ServiceRegisterTool.GetType(injection.InterfaceType);
                    var inplementationType = ServiceRegisterTool.GetType(injection.ImplementationType);

                    if (interfaceType != null && inplementationType != null && inplementationType.GetInterfaces().Any(c => c == interfaceType))
                    {
                        var lifetime = injection.Lifetime;
                        services.Add(new ServiceDescriptor(serviceType: interfaceType,
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
