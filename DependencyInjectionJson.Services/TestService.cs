using DependencyInjectionJson.IServices;
using DependencyInjectionJson.Repositories;

namespace DependencyInjectionJson.Services
{
    public class TestService : ITestService
    {
        private readonly ITestRepository _repository;

        public TestService(ITestRepository repository)
        {
            _repository = repository;
        }
        public string DoSomething(string apiMethod)
        {
            return this._repository.GetRepositoryInfo(apiMethod + "[TestService]");
        }
    }
}
