using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DependencyInjectionJson.XCutting
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AutoDIRegisterService(this IServiceCollection services, Action<object> options = null)
        {
            var register = new AutoDIRegisterService(services);
            register.RegisterAssemblies();

            return services;
        }
    }

    public class AutoDIRegisterService
    {
        private readonly Dictionary<string, AutoDIServiceInfo> _map;
        private readonly IServiceCollection _services;

        public AutoDIRegisterService(IServiceCollection services)
        {
            this._services = services;
            this._map = this.GetDependencyInjectionMap();
        }

        public void RegisterAssemblies(string fileName = "appsettings.json")
        {
            var jsonAssemblies = JObject.Parse(File.ReadAllText(fileName))["AutoDIRegisterService"]["assemblies"];
            if (jsonAssemblies != null)
            {
                var requiredAssemblies = JsonConvert.DeserializeObject<List<AutoDIAssemblieInfo>>(jsonAssemblies.ToString());
                foreach (var requiredAssembly in requiredAssemblies)
                {
                    if (requiredAssembly != null && !string.IsNullOrEmpty(requiredAssembly.Name))
                    {
                        try
                        {
                            this.RegisterFromAssembly(requiredAssembly.Name);
                        }catch(Exception ex)
                        {
                            Console.WriteLine($"Error trying to register '{requiredAssembly.Name}' assembly.", ex);
                            //Assembly problem. Not regitered.
                        }
                    }
                }
            }
        }

        private void RegisterFromAssembly(string assemblyName, string path = "")
        {
            assemblyName = assemblyName.Trim();
            var assemblyFileName = assemblyName;
            if (!assemblyName.ToUpper().EndsWith(".DLL"))
            {
                assemblyFileName += ".dll";
            }
            if(string.IsNullOrEmpty(path))
            {
                path = AppDomain.CurrentDomain.BaseDirectory;
            }
            string assemblyFile = Path.Combine(path, assemblyFileName);

            try
            {
                var assembly = System.Reflection.Assembly.LoadFrom(assemblyFile);

                if (!IsAssemblyLoaded(assemblyName))
                {
                    Microsoft.Extensions.DependencyModel.DependencyContextLoader.Default.Load(assembly);
                }
            }
            catch (System.IO.FileNotFoundException fnotfoundEx)
            {
                throw fnotfoundEx;
            }
            if(assemblyName.ToUpper().EndsWith(".DLL"))
            {
                assemblyName = assemblyName.Substring(0, assemblyName.Length - 4);
            }

            var injectionData = this.GetAssemblyInjetedDependencies(assemblyName);
            foreach (var injection in injectionData)
            {
                try
                {
                    var interfaceType = this.GetType(injection.Service);
                    var inplementationType = this.GetType(injection.Implementation);

                    if (interfaceType != null && inplementationType != null && inplementationType.GetInterfaces().Any(c => c == interfaceType))
                    {
                        var lifetime = injection.Lifetime;
                        _services.Add(new ServiceDescriptor(serviceType: interfaceType,
                                               implementationType: inplementationType,
                                               lifetime: lifetime));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error trying to register injection service: '{injection.Service} with implementation: '{injection.Implementation}'. Exception: {ex.Message}");
                }
            }
        }

        public List<AutoDIServiceInfo> GetAssemblyInjetedDependencies(string assemblyName)
        {
            var assembly = System.Reflection.Assembly.Load(assemblyName);

            List<AutoDIServiceInfo> resut = new List<AutoDIServiceInfo>();
            var interfaces = AppDomain.CurrentDomain.GetAssemblies()
                .Where(t => t.FullName.StartsWith(assemblyName))
                .FirstOrDefault()?.GetExportedTypes().Where(e => e.IsInterface)
                .Where(e => e.CustomAttributes.Any(c => c.AttributeType == typeof(AutoDIServiceAttributeAttribute)));
            if(interfaces!=null)
            {
                foreach (var interficie in interfaces)
                {
                    var service = this.GetServiceInfoFromMap(interficie);
                    if(service==null)
                    {
                        var interfaceTypeName = interficie.FullName;
                        var implementationTypeName = this.GetImplementationTypeFromParameters(interficie);
                        if(string.IsNullOrEmpty(implementationTypeName))
                        {
                            implementationTypeName = this.GetDefaultImplementationType(interficie);
                        }

                        var lifetimeType = ServiceLifetime.Transient;

                        var lifetime = this.GetLifetimeFromParameters(interficie);
                        if(lifetime!=null)
                        {
                            lifetimeType = (ServiceLifetime)lifetime;
                        }

                        service = new AutoDIServiceInfo(
                            service: interfaceTypeName,
                            implementation: implementationTypeName,
                            lifetime: lifetimeType);
                    }
                    resut.Add(service);
                }
            }

            return resut;
        }

        private AutoDIServiceInfo GetServiceInfoFromMap(Type interficie)
        {
            AutoDIServiceInfo result = null;
            if(interficie!=null)
            {
                var interfaceTypeName = interficie.FullName;
                if (_map != null && _map.ContainsKey(interfaceTypeName))
                {
                    result = _map[interfaceTypeName];
                }
            }
            return result;
        }

        private string GetImplementationTypeFromParameters(Type interficie)
        {
            var implementationTypeName = "";
            var customAttr = interficie.CustomAttributes.SingleOrDefault(c => c.AttributeType == typeof(AutoDIServiceAttributeAttribute));
            if(customAttr==null)
            {
                throw new Exception($"Interface '{interficie.FullName}' doesn't has a 'ServiceImplementationAttribute' CustomAttibute.");
            }
            var implementationArgument = customAttr.ConstructorArguments[0].Value as string;
            if (!string.IsNullOrEmpty(implementationArgument))
            {
                var interfaceTypeName = interficie.FullName;
                var interfaceName = interficie.FullName.Split(".").Last();
                var interfaceAssemblyName = interficie.FullName.Substring(0, interficie.FullName.Length - (interfaceName.Length + 1));

                if (implementationArgument.Contains('.'))
                {
                    implementationTypeName = implementationArgument;
                }
                else
                {
                    implementationTypeName = interfaceAssemblyName + "." + implementationArgument;
                }
            }
            return implementationTypeName;
        }

        private string GetDefaultImplementationType(Type interficie)
        {
            var implementationTypeName = "";

            var interfaceTypeName = interficie.FullName;
            var name = interficie.FullName.Split(".").Last();
            var interfaceAssemblyName = interficie.FullName.Substring(0, interficie.FullName.Length - (name.Length + 1));
            implementationTypeName = interfaceAssemblyName + "." + name.Substring(1);

            return implementationTypeName;
        }

        private int? GetLifetimeFromParameters(Type interficie)
        {
            var customAttr = interficie.CustomAttributes.SingleOrDefault(c => c.AttributeType == typeof(AutoDIServiceAttributeAttribute));
            if (customAttr == null)
            {
                throw new Exception($"Interface '{interficie.FullName}' doesn't has a 'ServiceImplementationAttribute' CustomAttibute.");
            }
            if(customAttr.ConstructorArguments.Count>1)
            {
                return (int)customAttr.ConstructorArguments[1].Value;
            }else
            {
                return null;
            }
            
        }

        private Dictionary<string, AutoDIServiceInfo> GetDependencyInjectionMap(string fileName = "appsettings.json")
        {
            Dictionary<string, AutoDIServiceInfo> _map = new Dictionary<string, AutoDIServiceInfo>();

            var jsonServices = JObject.Parse(File.ReadAllText(fileName))["AutoDIRegisterService"]["services"];
            if (jsonServices != null)
            {
                var requiredServices = JsonConvert.DeserializeObject<List<AutoDIServiceInfo>>(jsonServices.ToString());

                foreach (var service in requiredServices)
                {
                    if (_map.ContainsKey(service.Service))
                    {
                        _map[service.Service] = service;
                    }
                    else
                    {
                        _map.Add(service.Service, service);
                    }
                }
            }

            return _map;
        }

        public Type GetType(string typename)
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
                throw new Exception($"Type: '{typename}' not found.");
            }
            return result;
        }

        private bool IsAssemblyLoaded(string assemblyName)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(t => t.FullName.StartsWith(assemblyName))
                .FirstOrDefault()==null ? false : true;
        }
    }

    public class AutoDIAssemblieInfo
    {
        public string Name { get; set; }

        public AutoDIAssemblieInfo()
        {

        }

        public AutoDIAssemblieInfo(string name)
        {
            this.Name = name;
        }
    }

    public class AutoDIServiceInfo
    {
        public string Service { get; set; }

        public string Implementation { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceLifetime Lifetime { get; set; }

        public AutoDIServiceInfo()
        {

        }

        public AutoDIServiceInfo(string service, string implementation, ServiceLifetime lifetime)
        {
            this.Service = service;
            this.Implementation = implementation;
            this.Lifetime = lifetime;
        }
    }

    public class AutoDIServiceAttributeAttribute : Attribute
    {
        private readonly string implementationType;
        private readonly ServiceLifetime lifetime = ServiceLifetime.Transient;

        public AutoDIServiceAttributeAttribute(string implementationType = "", ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            this.implementationType = implementationType;
            this.lifetime = lifetime;
        }

        public virtual string ImplementationType
        {
            get { return this.implementationType; }
        }

        public virtual ServiceLifetime Lifetime
        {
            get { return this.lifetime; }
        }
    }

}
