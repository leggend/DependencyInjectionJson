using DependencyInjectionJson.XCutting;

namespace DependencyInjectionJson.IServices
{
    [AutoDIServiceAttribute(implementationType: "DependencyInjectionJson.Services.TestService")]
    public interface ITestService
    {
        string DoSomething(string apiMethod);
    }
}
