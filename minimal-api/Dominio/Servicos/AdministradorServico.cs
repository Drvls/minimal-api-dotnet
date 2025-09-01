using System.Linq;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Interfaces;
using minimal_api.Infraestrutura.Db;

namespace minimal_api.Dominio.Servicos;

public class AdministradorServico : IAdministradorServico
{
    private readonly DbContexto _dbContexto;
    public AdministradorServico(DbContexto contexto)
    {
        _dbContexto = contexto;
    }
    
    public Administrador? Login(LoginDTO login) =>
        _dbContexto.Administradores
            .Where(a => a.Email == login.Email && a.Senha == login.Senha)
            .FirstOrDefault();
}