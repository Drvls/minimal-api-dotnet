using minimal_api.Dominio.Entidades;

namespace teste_minimal_api.Domain.Entity;

[TestClass]
public class AdministradorTeste
{
    [TestMethod]
    public void TestGetSetProperties()
    {
        // Arrange > variáveis para validação
        var admin = new Administrador();
        
        // Act > Ação
        admin.Id = 1;
        admin.Email = "Teste@TesteUnitario.org";
        admin.Senha = "TestandoUnitariamente";
        admin.Perfil = "Admin";

        // Assert > Validação
        Assert.AreEqual(1, admin.Id);
        Assert.AreEqual("Teste@TesteUnitario.org", admin.Email);
        Assert.AreEqual("TestandoUnitariamente",  admin.Senha);
        Assert.AreEqual("Admin",  admin.Perfil);
    }
}