using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DependencyInjectionJson
{
    public class InjectedService
    {
        public string ServiceType { get; set; }

        public string ImplementationType { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceLifetime Lifetime { get; set; }
    }
}
