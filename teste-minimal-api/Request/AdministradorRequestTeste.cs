using System.Net;
using System.Text;
using System.Text.Json;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.ModelViews;
using teste_minimal_api.Helpers;

namespace teste_minimal_api.Request;

[TestClass]
public class AdministradorRequestTeste
{
    [ClassInitialize]
    public static void ClassInit(TestContext testContext)
    {
        Setup.ClassInit(testContext);
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        Setup.ClassCleanup();
    }
    
    [TestMethod]
    public async Task TestGetSetProperties()
    {
        // Arrange
        var loginDTO = new LoginDTO
        {
            Email = "admin@email",
            Senha = "admin123",
        };

        var content = new StringContent(JsonSerializer.Serialize(loginDTO), 
            Encoding.UTF8, "application/json");
        
        // Act
        var response = await Setup.client.PostAsync("/administradores/login", content);
        
        // Assert
        Assert.AreEqual(HttpStatusCode.OK,  response.StatusCode);
        
        var result = await response.Content.ReadAsStringAsync();
        var adminLogado =  JsonSerializer.Deserialize<AdminLogado>(result, new  JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        Assert.IsNotNull(adminLogado.Email);
        Assert.IsNotNull(adminLogado.Perfil);
        Assert.IsNotNull(adminLogado.Token);
    }
}