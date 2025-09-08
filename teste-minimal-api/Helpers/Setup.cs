using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using minimal_api;
using minimal_api.Dominio.Interfaces;
using teste_minimal_api.Mocks;

namespace teste_minimal_api.Helpers;

public class Setup
{
    public const string PORT = "5001";
    public static TestContext TestContext = default;
    public static WebApplicationFactory<Startup> http = default;
    public static HttpClient client = default;

    public static void ClassInit(TestContext testContext)
    {
        Setup.TestContext = testContext;
        Setup.http = new WebApplicationFactory<Startup>();

        Setup.http = Setup.http.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("https_port", Setup.PORT).UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                services.AddScoped<IAdministradorServico, AdministradorServicoMock>();
            });
        });
        
        Setup.client = Setup.http.CreateClient();
    }

    public static void ClassCleanup()
    {
        Setup.http.Dispose();
    }
}