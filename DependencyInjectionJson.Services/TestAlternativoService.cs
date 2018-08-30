using DependencyInjectionJson.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInjectionJson.Services
{
    public class TestAlternativoService : ITestService
    {
        private readonly ITestRepository _repository;

        public TestAlternativoService(ITestRepository repository)
        {
            _repository = repository;
        }
        public string DoSomething(string apiMethod)
        {
            return this._repository.GetRepositoryInfo(apiMethod + "[TestAlternativoService]");
        }
    }
}
