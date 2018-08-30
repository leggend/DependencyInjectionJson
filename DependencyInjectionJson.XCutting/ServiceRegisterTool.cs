using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DependencyInjectionJson.XCutting
{
    public class ServiceRegisterTool
    {
        public static List<DependencyInjectionInfo> GetAssemblyIncetedDependencies(string assemblyName, Dictionary<string, DependencyInjectionInfo> map)
        {
            List<DependencyInjectionInfo> resut = new List<DependencyInjectionInfo>();
            var interfaces = AppDomain.CurrentDomain.GetAssemblies()
                .Where(t => t.FullName.StartsWith(assemblyName))
                .FirstOrDefault().GetExportedTypes().Where(e => e.IsInterface)
                .Where(e => e.CustomAttributes.Any(c => c.AttributeType == typeof(ServiceImplementationAttribute)));

            foreach (var interficie in interfaces)
            {
                var customAttr = interficie.CustomAttributes.SingleOrDefault(c => c.AttributeType == typeof(ServiceImplementationAttribute));
                var serviceTypeName = interficie.FullName;
                var name = interficie.FullName.Split(".").Last();
                var interfaceAssemblyName = interficie.FullName.Substring(0, interficie.FullName.Length - (name.Length + 1));
                var implementationTypeName = interfaceAssemblyName + "." + name.Substring(1);
                var lifetimeType = ServiceLifetime.Transient;

                var impleArg = customAttr?.ConstructorArguments[0].Value as string;
                if (!string.IsNullOrEmpty(impleArg))
                {
                    if (impleArg.Contains('.'))
                    {
                        implementationTypeName = impleArg;
                    }
                    else
                    {
                        implementationTypeName = interfaceAssemblyName + "." + impleArg;
                    }
                }

                var lifetimeArg = customAttr.ConstructorArguments[1].Value;
                if (lifetimeArg != null)
                {
                    lifetimeType = (ServiceLifetime)lifetimeArg;
                }

                DependencyInjectionInfo service = new DependencyInjectionInfo(serviceType: serviceTypeName, implementationType: implementationTypeName, lifetime: lifetimeType);
                if(map.ContainsKey(serviceTypeName))
                {
                    service = map[serviceTypeName];
                }

                resut.Add(service);
            }
            return resut;
        }

        public static Dictionary<string, DependencyInjectionInfo> GetDependencyInjectionMap(string fileName = "appsettings.json")
        {
            Dictionary<string, DependencyInjectionInfo> _map = new Dictionary<string, DependencyInjectionInfo>();

            var jsonServices = JObject.Parse(File.ReadAllText(fileName))["services"];
            if (jsonServices != null)
            {
                var requiredServices = JsonConvert.DeserializeObject<List<DependencyInjectionInfo>>(jsonServices.ToString());

                foreach (var service in requiredServices)
                {
                    if (_map.ContainsKey(service.ServiceType))
                    {
                        _map[service.ServiceType] = service;
                    }
                    else
                    {
                        _map.Add(service.ServiceType, service);
                    }
                }
            }

            return _map;
        }

    }
}
