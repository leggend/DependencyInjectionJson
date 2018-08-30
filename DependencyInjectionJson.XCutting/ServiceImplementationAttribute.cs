using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DependencyInjectionJson.XCutting
{
    public class ServiceImplementationAttribute : Attribute
    {
        private readonly string implementationType;
        private readonly ServiceLifetime lifetime = ServiceLifetime.Transient;

        public ServiceImplementationAttribute(string implementationType = "", ServiceLifetime  lifetime = ServiceLifetime.Transient)
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
