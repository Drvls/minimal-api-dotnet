using System.Collections.Generic;
using System.Linq;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Interfaces;
using minimal_api.Infraestrutura.Db;

namespace minimal_api.Dominio.Servicos;

public class AdministradorServico : IAdministradorServico
{
    private readonly DbContexto _contexto;
    public AdministradorServico(DbContexto contexto)
    {
        _contexto = contexto;
    }
    
    public Administrador? Login(LoginDTO login) =>
        _contexto.Administradores
            .Where(a => a.Email == login.Email && a.Senha == login.Senha)
            .FirstOrDefault();

    public Administrador Incluir(Administrador administrador)
    {
        _contexto.Administradores.Add(administrador);
        _contexto.SaveChanges();
        
        return administrador;
    }

    public List<Administrador> Todos(int? pagina)
    {
        var query = _contexto.Administradores.AsQueryable();

        int itensPorPagina = 10;

        if (pagina != null)
        {
            query = query.Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);
        }
        
        return query.ToList();
    }

    public Administrador? BuscarPorID(int id)
    {
        return _contexto.Administradores.Where(v => v.Id == id).FirstOrDefault();
    }
}