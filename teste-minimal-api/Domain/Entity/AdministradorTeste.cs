using minimal_api.Dominio.Entidades;

namespace teste_minimal_api.Domain.Entity;

[TestClass]
public class AdministradorTeste
{
    [TestMethod]
    public void TestGetSetProperties()
    {
        // Arrange > variáveis para validação
        var admin = new Administrador
        {
            // Act > Ação
            Id = 1,
            Email = "Teste@TesteUnitario.org",
            Senha = "Testando",
            Perfil = "Admin"
        };

        // Assert > Validação
        Assert.AreEqual(1, admin.Id);
        Assert.AreEqual("Teste@TesteUnitario.org", admin.Email);
        Assert.AreEqual("Testando",  admin.Senha);
        Assert.AreEqual("Admin",  admin.Perfil);
    }
}