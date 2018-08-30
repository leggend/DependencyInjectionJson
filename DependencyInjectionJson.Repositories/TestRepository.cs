using System;

namespace DependencyInjectionJson.Repositories
{
    public class TestRepository : ITestRepository
    {
        public string GetRepositoryInfo(string serviceName)
        {
            return serviceName + "[TestRepository] - Hola desde el repositorio.";
        }
    }
}
