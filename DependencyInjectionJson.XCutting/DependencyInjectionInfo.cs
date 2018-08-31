using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace DependencyInjectionJson.XCutting
{
    public class DependencyInjectionInfo
    {
        public string InterfaceType { get; set; }

        public string ImplementationType { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceLifetime Lifetime { get; set; }

        public DependencyInjectionInfo()
        {

        }

        public DependencyInjectionInfo(string interfaceType, string implementationType, ServiceLifetime lifetime)
        {
            this.InterfaceType = interfaceType;
            this.ImplementationType = implementationType;
            this.Lifetime = lifetime;
        }
    }
}
