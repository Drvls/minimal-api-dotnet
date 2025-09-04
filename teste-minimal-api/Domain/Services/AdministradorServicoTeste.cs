using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Servicos;
using minimal_api.Infraestrutura.Db;

namespace teste_minimal_api.Domain.Services;

[TestClass]
public class AdministradorServicoTeste
{
    private DbContexto CriarContexto()
    {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));

        var builder = new ConfigurationBuilder()
            .SetBasePath(path)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();
        
        var config = builder.Build();
        return  new DbContexto(config);
    }
    
    [TestMethod]
    public void TesteSalvarAdministrador()
    {
        // Arrange
        var context = CriarContexto();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE administradores");
        
        var admin = new Administrador
        {
            Id = 1,
            Email = "Teste@TesteUnitario.org",
            Senha = "Testando",
            Perfil = "Admin"
        };
        var adminServico = new AdministradorServico(context);
        
        // Act
        adminServico.Incluir(admin);
        
        // Assert
        Assert.AreEqual(1, adminServico.Todos(1).Count());
    }
    
    [TestMethod]
    public void TesteBuscaAdministrador()
    {
        // Arrange
        var context = CriarContexto();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE administradores");
        
        var admin = new Administrador
        {
            Id = 1,
            Email = "Teste@TesteUnitario.org",
            Senha = "Testando",
            Perfil = "Admin"
        };
        var adminServico = new AdministradorServico(context);
        
        // Act
        adminServico.Incluir(admin);
        var adminId = adminServico.BuscarPorID(admin.Id);
        
        // Assert
        Assert.AreEqual(1, adminId.Id);
    }
    
    [TestMethod]
    public void TesteTodosAdministrador()
    {
        // Arrange
        var context = CriarContexto();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE administradores");
        
        var admin = new Administrador
        {
            Id = 1,
            Email = "Teste@TesteUnitario.org",
            Senha = "Testando",
            Perfil = "Admin"
        };
        var adminServico = new AdministradorServico(context);
        
        // Act
        adminServico.Incluir(admin);
        
        // Assert
        Assert.AreEqual(1, adminServico.Todos(1).Count);
    }
}