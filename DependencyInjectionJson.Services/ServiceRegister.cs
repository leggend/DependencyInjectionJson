using DependencyInjectionJson.XCutting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using DependencyInjectionJson.IServices;
using System.Reflection;

namespace DependencyInjectionJson.Services
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection RegisterDependencyInjectionServices(this IServiceCollection services, Dictionary<string, DependencyInjectionInfo> map)
        {
            var assemblyName = "DependencyInjectionJson.IServices";
            var assembly = Assembly.Load(assemblyName);
            Microsoft.Extensions.DependencyModel.DependencyContextLoader.Default.Load(assembly);

            var injectionData = ServiceRegisterTool.GetAssemblyInjetedDependencies(assemblyName, map);
            foreach (var injection in injectionData)
            {
                try
                {
                    var interfaceType = ServiceRegisterTool.GetType(injection.InterfaceType);
                    var inplementationType = ServiceRegisterTool.GetType(injection.ImplementationType);

                    if (interfaceType != null && inplementationType!=null && inplementationType.GetInterfaces().Any(c => c == interfaceType))
                    {
                        var lifetime = injection.Lifetime;
                        services.Add(new ServiceDescriptor(serviceType: interfaceType,
                                               implementationType: inplementationType,
                                               lifetime: lifetime));
                    }
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
            return services;
        }
    }
}
