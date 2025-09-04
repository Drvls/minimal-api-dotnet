using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Servicos;
using minimal_api.Infraestrutura.Db;

namespace teste_minimal_api.Domain.Services;

[TestClass]
public class VeiculoServicoTest
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
    public void TesteSalvarVeiculo()
    {
        // Arrange
        var context = CriarContexto();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE veiculos");

        var vtr = new Veiculo
        {
            Id = 1,
            Nome = "Monza SL",
            Marca = "Chevrolet",
            Ano = 1984
        };

        var vtrServ = new VeiculoServico(context);
        
        // Act
        vtrServ.Incluir(vtr);
        
        // Assert
        Assert.AreEqual(1, vtrServ.Todos(1).Count());
    }
    
    [TestMethod]
    public void TesteBuscaVeiculo()
    {
        // Arrange
        var context = CriarContexto();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE veiculos");

        var vtr = new Veiculo
        {
            Id = 1,
            Nome = "Monza SL",
            Marca = "Chevrolet",
            Ano = 1984
        };

        var vtrServ = new VeiculoServico(context);
        
        // Act
        vtrServ.Incluir(vtr);
        var vtrId = vtrServ.BuscaPorId(vtr.Id);
        
        // Assert
        Assert.AreEqual(1, vtrId.Id);
    }
    
    public void TesteTodosVeiculo()
    {
        // Arrange
        var context = CriarContexto();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE veiculos");

        var vtr = new Veiculo
        {
            Id = 1,
            Nome = "Monza SL",
            Marca = "Chevrolet",
            Ano = 1984
        };

        var vtrServ = new VeiculoServico(context);
        
        // Act
        vtrServ.Incluir(vtr);
        
        // Assert
        Assert.AreEqual(1, vtrServ.Todos(1).Count());
    }

    
    [TestMethod]
    public void TesteAtualizarVeiculo()
    {
        // Arrange
        var context = CriarContexto();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE veiculos");

        var vtr = new Veiculo
        {
            Id = 1,
            Nome = "Monza SL",
            Marca = "Chevrolet",
            Ano = 1984
        };

        var vtr2 = new Veiculo
        {
            Nome = "Uno Mille",
            Marca = "Fiat",
            Ano = 1990
        };
        
        var vtrServ = new VeiculoServico(context);
        
        // Act
        vtrServ.Incluir(vtr);
        vtrServ.Atualizar(vtr2);
        
        // Assert
        Assert.AreEqual(1990, vtr2.Ano);
    }
    
    [TestMethod]
    public void TesteExcluirVeiculo()
    {
        // Arrange
        var context = CriarContexto();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE veiculos");

        var vtr = new Veiculo
        {
            Id = 1,
            Nome = "Monza SL",
            Marca = "Chevrolet",
            Ano = 1984
        };
        
        var vtrServ = new VeiculoServico(context);
        
        // Act
        vtrServ.Incluir(vtr);
        vtrServ.Excluir(vtr);
        
        // Assert
        Assert.AreEqual(1, vtr.Id);
        Assert.AreEqual(0, vtrServ.Todos(1).Count);
    }
}