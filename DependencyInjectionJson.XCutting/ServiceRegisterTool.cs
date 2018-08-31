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
        public static List<DependencyInjectionInfo> GetAssemblyInjetedDependencies(string assemblyName, Dictionary<string, DependencyInjectionInfo> map)
        {
            var assembly = System.Reflection.Assembly.Load(assemblyName);
            if (!IsAssemblyLoaded(assemblyName))
            {
                Microsoft.Extensions.DependencyModel.DependencyContextLoader.Default.Load(assembly);
            }

            List<DependencyInjectionInfo> resut = new List<DependencyInjectionInfo>();
            var interfaces = AppDomain.CurrentDomain.GetAssemblies()
                .Where(t => t.FullName.StartsWith(assemblyName))
                .FirstOrDefault()?.GetExportedTypes().Where(e => e.IsInterface)
                .Where(e => e.CustomAttributes.Any(c => c.AttributeType == typeof(ServiceImplementationAttribute)));

            foreach (var interficie in interfaces)
            {
                var customAttr = interficie.CustomAttributes.SingleOrDefault(c => c.AttributeType == typeof(ServiceImplementationAttribute));
                var interfaceTypeName = interficie.FullName;
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

                DependencyInjectionInfo service = new DependencyInjectionInfo(interfaceType: interfaceTypeName, implementationType: implementationTypeName, lifetime: lifetimeType);
                if(map.ContainsKey(interfaceTypeName))
                {
                    service = map[interfaceTypeName];
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
                    if (_map.ContainsKey(service.InterfaceType))
                    {
                        _map[service.InterfaceType] = service;
                    }
                    else
                    {
                        _map.Add(service.InterfaceType, service);
                    }
                }
            }

            return _map;
        }

        public static Type GetType(string typename)
        {
            Type result = null;
            if (typename.Contains("."))
            {
                var name = typename.Split('.').Last();
                var assemblyName = typename.Substring(0, typename.Length - (name.Length + 1));
                var assembly = System.Reflection.Assembly.Load(assemblyName);
                if (assembly != null)
                {
                    result = assembly.GetType(typename);
                } else
                {
                    throw new Exception($"Assembly: '{assemblyName}' not found.");
                }
            } else
            {
                result = Type.GetType(typename);
            }

            if(result==null)
            {
                //Type not found
                throw new Exception($"Type: '{typename}' not found.");
            }
            return result;
        }

        private static bool IsAssemblyLoaded(string assemblyName)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(t => t.FullName.StartsWith(assemblyName))
                .FirstOrDefault()==null ? false : true;
        }

    }
}
