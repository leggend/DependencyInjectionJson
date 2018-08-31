# DependencyInjectionJson
Esta solución permite autoregistrar la inyeccion de dependencias de los assemblies de nuestro proyecto.

Para ello, he creado un CustomAttribute "**ServiceImplementation**" que pondremos en las Interficies que queramos autoregistrar.

Los parámetros soportados por este CustomAttribute son:
+ **implementationType:** Nombre (puede incluir o no el namespace) de la clase que se registrará para implementar la interficio.
+ **lifetime:** ServiceLifetime con el que se ha de registrar la dependencia.

Si no epecificamos ningun "implementatioType", se intentará registrar la clase con el mismo namespace y nombre (sin la primera letra "I") de la Interficie.
Ejemplo:
```csharp
[ServiceImplementation()]
public interface ITestService {
}

public class TestService: ITestService {
}
```

Podemos especificar una implentacion alternativa mediante el parámetro "**implementationType**":
```csharp
[ServiceImplementation(implementationType: "TestAlternativeService")]
public interface ITestService {
}

public class TestAlternativeService: ITestService {
}
```

Ademas, podemos definir en un fichero JSON (por defecto se busca en '**appsettings.json**'), las dependencias, teniendo estas prioridad sobre las definidas mediante el CusttomAttribute.
El formator del fichero JSON es el siguiente:
```json
{
    "services": [
        {
            "serviceType": "DependencyInjectionJson.Services.ITestService",
            "implementationType": "DependencyInjectionJson.Services.TestTerceroService",
            "lifetime": "Transient"
        }
    ]
}
```
Para autoregistras la inyección de dependencias de nuestros assemblies, hemos de crear una clase '**ServiceRegister.cs**' en nuestra proyecto web, donde registraremos la injección de dependencias de todos nuestros assemblies:
```csharp
public static class IServiceCollectionExtensions
{
    public static IServiceCollection RegisterDependencyInjections(this IServiceCollection services)
    {
        var map = ServiceRegisterTool.GetDependencyInjectionMap();
        services = ServiceRegisterTool.RegisterDependencyInjectionAssembly(services, "DependencyInjectionJson.Repositories", map);
        services = ServiceRegisterTool.RegisterDependencyInjectionAssembly(services, "DependencyInjectionJson.IServices", map);
        return services;
    }
}
```

Finalmente, en el fichero startup.cs, solo tenemos que invocar el servicio de registro que hemos creado.
```csharp
    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.RegisterDependencyInjections();
        services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
    }

```
