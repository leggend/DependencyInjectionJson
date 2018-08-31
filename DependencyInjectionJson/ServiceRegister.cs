using DependencyInjectionJson.XCutting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DependencyInjectionJson
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection RegisterDependencyInjections(this IServiceCollection services)
        {
            var map = ServiceRegisterTool.GetDependencyInjectionMap();
            services = ServiceRegisterTool.RegisterDependencyInjectionAssembly(services, "DependencyInjectionJson.Repositories", map);
            services = ServiceRegisterTool.RegisterDependencyInjectionAssembly(services, "DependencyInjectionJson.IServices", map);
            return services;
        }
    }
}
