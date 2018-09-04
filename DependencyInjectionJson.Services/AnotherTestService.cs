using DependencyInjectionJson.IServices;
using DependencyInjectionJson.Repositories;

namespace DependencyInjectionJson.Services
{
    public class AnotherTestService : ITestService
    {
        private readonly ITestRepository _repository;

        public AnotherTestService(ITestRepository repository)
        {
            _repository = repository;
        }
        public string DoSomething(string apiMethod)
        {
            return this._repository.GetRepositoryInfo(apiMethod + "[AnotherTestService]");
        }
    }
}
