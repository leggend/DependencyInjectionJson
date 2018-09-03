# DependencyInjectionJson
Esta solución permite autoregistrar la inyeccion de dependencias de los assemblies de nuestro proyecto, mediante el servicio **AutoDIRegisterService**.

Para ello, he creado un *CustomAttribute* "**ServiceImplementation**" que pondremos en las Interficies que queramos **AutoDIRegisterService** registre.

Los parámetros soportados por este *CustomAttribute* son:
+ **implementationType:** Nombre (puede incluir o no el namespace) de la clase que se registrará para implementar la interficio.
+ **lifetime:** ServiceLifetime con el que se ha de registrar la dependencia.

Si no epecificamos ningun "**implementatioType**", se intentará registrar la clase con el mismo namespace y nombre (sin la primera letra "I") de la Interficie.
Ejemplo:
```csharp
[AutoDIServiceAttribute()]
public interface ITestService {
}

public class TestService: ITestService {
}
```

Podemos especificar una implentacion alternativa mediante el parámetro "**implementationType**":
```csharp
[AutoDIServiceAttribute(implementationType: "TestAlternativeService")]
public interface ITestService {
}

public class TestAlternativeService: ITestService {
}
```

En un fichero JSON (por defecto se busca en '**appsettings.json**'), definiremos las lista de assemblies en los que el servicio de Autoregistro de dependencias, deberá buscar Interficies con el CustomAttribute "" para proceder a su registro. esta lista de Assemblies estará definida en el nodo "*AutoDIRegisterService->assemblies*"
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

Ademas, en este fichero JSON, podemos definir, agregar mnuevas depenedecias a registrar o incluso sobreescribir algunas de las definidas mediante el *CusttomAtrribute*. Estas dependencias definidas en el fichero JSON, tendrán prioridad sobre las definidas por los *CustomAttribute* de cada Interficie. 
Estas dependendencias adicionales/alternativas se definen en el nodo "*AutoDIRegisterService->services*" del fichero JSON.
El formator del fichero JSON es el siguiente:
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
          "implementation": "DependencyInjectionJson.Services.TestTerceroService",
          "lifetime": "Transient"
        }
      ]
    }
}
```


Finalmente, en el fichero startup.cs, solo tenemos que invocar el servicio de registro que hemos creado.
```csharp
    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AutoDIRegisterService();
        services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
    }

```
