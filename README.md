# DependencyInjectionJson
This solution implements the service "**AutoDIRegisterService**", which will be responsible for self-registration in the Net Core Dependency Injection framework, the implementations of our assemblies.

**AutoDIServiceAttribute** *CustomAttribute* has been created to decorate our Interfaces with needed information so that **AutoDIRegisterService** can register them in Net Core Dependency Injection framework.

These are the **AutoDIServiceAttribute** arguments:
+ **implementationType** (optional): Implementation class name (with or without namespace).
+ **lifetime** (optional): Service Lifetime for registration.

If no "**implementatioType**" is defined, **AutoDIRegisterService** will try to register an implementation class name with the same namespace and name without first char (normally 'I').
Example:
```csharp
[AutoDIServiceAttribute()]
public interface ITestService {
}

public class TestService: ITestService {
}
```

We can specify an alternative name for interface implementation.
If namespace is defined, **AutoDIRegisterService** will try to register with same Interface's namespace.
```csharp
[AutoDIServiceAttribute(implementationType: "TestAlternativeService")]
public interface ITestService {
}

public class TestAlternativeService: ITestService {
}
```
**AutoDIRegisterService** use a JSON file ('**appsettings. json**' by default), to define which assemblies must be self-registered.
```json
{
    "AutoDIRegisterService": {
        "assemblies": [
        {
            "name": "DependencyInjectionJson.Repositories.dll"

        },
        {
            "name": "DependencyInjectionJson.IServicesZZ.dll"
        },
        {
            "name": "DependencyInjectionJson.IServices.dll"
        }
        ],
    }
}
```

Additionally, in than JSON file, we can specify custom dependencies, which will have priority over CustomAttribute definition.
```json
{
    "AutoDIRegisterService": {
        "assemblies": [
            {
                "name": "Infomed.IKernel.dll"
            },
            {
                "name": "Infomed.Kernel.dll"
            },
            {
                "name": "Gesden.Repositories.dll"
            }
        ],
      "services": [
        {
          "service": "DependencyInjectionJson.IServices.ITestService",
          "implementation": "DependencyInjectionJson.Services.AnotherTestService",
          "lifetime": "Transient"
        }
      ]
    }
}
```

To use **AutoDIRegisterService** in outo solution, we have to register as other services in *Startup. cs* *ConfigureServices* method.
```csharp
    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AutoDIRegisterService();
        services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
    }

```
