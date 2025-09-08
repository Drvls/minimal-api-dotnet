using minimal_api.Dominio.Entidades;

namespace teste_minimal_api.Domain.Entity;

[TestClass]
public class VeiculoTeste
{
    [TestMethod]
    public void TestGetSetProperties()
    {
        var car = new Veiculo
        {
            Id = 1,
            Nome = "Monza SL",
            Marca = "Chevrolet",
            Ano = 1984
        };

        Assert.AreEqual(1, car.Id);
        Assert.AreEqual("Monza SL", car.Nome);
        Assert.AreEqual("Chevrolet", car.Marca);
        Assert.AreEqual(1984, car.Ano);
    }
}