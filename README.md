# DependencyInjectionJson
Se ha creado el CustomAttribute "**ServiceImplementation**" para definir la información necerasria para poder registrar Interficies en el servicio de Inyección de Dependencias de ASP.NET Core.

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


Podemos especificar una implentacion alternativa mediante el parámetro "implementationType":
```csharp
[ServiceImplementation(implementationType: "TestAlternativeService")]
public interface ITestService {
}

public class TestAlternativeService: ITestService {
}
```

Ademas de la información de registro definida mediante el CustomAttribute, tambien podemos especificar en un fichero JSON (por defecto "appsettings.json").

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
Los registros especificados en este archivo tienen prioridad sobre las implementaciones definidas mediante el CustomAttribute.

Para regustrar automaticamente las implementaciones de cada Assembliby crearemos un servicio de registro en cada uno.
(Recordar cambiar el nombre del método de registro en cada assembly)
```csharp
public static class IServiceCollectionExtensions
{
    public static IServiceCollection RegisterDependencyInjectionServices(this IServiceCollection services, Dictionary<string, DependencyInjectionInfo> map)
    {
        var injectionData = ServiceRegisterTool.GetAssemblyIncetedDependencies("DependencyInjectionJson.Services", map);
        foreach (var injection in injectionData)
        {
            try
            {
                var serviceType = Type.GetType(injection.ServiceType);
                var inplementationType = Type.GetType(injection.ImplementationType);
                
                if(serviceType!=null && inplementationType!=null && inplementationType.GetInterfaces().Any(c => c == serviceType))
                {
                    var lifetime = injection.Lifetime;
                    services.Add(new ServiceDescriptor(serviceType: serviceType,
                                            implementationType: inplementationType,
                                            lifetime: lifetime));
                }
            }
            catch(Exception ex)
            {
                //Posiblemente no se encuentra algun tipo
            }
        }
        return services;
    }
}
```

Finalmente, en el fichero startup.cs recogeremos el "mapa" de registro de dependencias definido en el archivo JSON, y procederemos a llamar al registro de dependencias de cada uno de nuestros assemblies

```csharp
    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        //Get map of dependeny injection
        var map = ServiceRegisterTool.GetDependencyInjectionMap();
        services.RegisterDependencyInjectionRepositories(map);
        services.RegisterDependencyInjectionServices(map);

        services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
    }

```
