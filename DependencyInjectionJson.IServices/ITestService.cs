using DependencyInjectionJson.XCutting;

namespace DependencyInjectionJson.IServices
{
    [ServiceImplementation(implementationType: "DependencyInjectionJson.Services.TestService")]
    public interface ITestService
    {
        string DoSomething(string apiMethod);
    }
}
