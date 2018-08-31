using DependencyInjectionJson.XCutting;
using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInjectionJson.Repositories
{
    [AutoDIServiceAttribute()]
    public interface ITestRepository
    {
        string GetRepositoryInfo(string serviceName);
    }
}
