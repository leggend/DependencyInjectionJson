using DependencyInjectionJson.XCutting;
using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInjectionJson.Repositories
{
    [ServiceImplementation()]
    public interface ITestRepository
    {
        string GetRepositoryInfo(string serviceName);
    }
}
