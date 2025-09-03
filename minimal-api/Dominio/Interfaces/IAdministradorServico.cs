using System.Collections.Generic;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;

namespace minimal_api.Dominio.Interfaces;

public interface IAdministradorServico
{
    Administrador? Login(LoginDTO login);
    Administrador Incluir(Administrador administrador);
    List<Administrador> Todos(int? pagina);
    
    Administrador? BuscarPorID(int id);
}