using DependencyInjectionJson.XCutting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DependencyInjectionJson.Services
{
    [ServiceImplementation(implementationType: "TestAlternativoService")]
    public interface ITestService
    {
        string DoSomething(string apiMethod);
    }
}
