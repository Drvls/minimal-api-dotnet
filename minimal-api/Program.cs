using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Enuns;
using minimal_api.Dominio.Interfaces;
using minimal_api.Dominio.ModelViews;
using minimal_api.Dominio.Servicos;
using minimal_api.Infraestrutura.Db;

#region Builder

var builder = WebApplication.CreateBuilder(args);
var key = builder.Configuration.GetSection("Jwt").ToString();

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateAudience = false,
        ValidateIssuer = false
    };
});


builder.Services.AddAuthorization();
builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

builder.Services.AddDbContext<DbContexto>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
        );
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
        }
    });
});

var app = builder.Build();
app.UseHttpsRedirection();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Administradores

string GenerateTokenJwt(Administrador administrador)
{
    if (string.IsNullOrEmpty(key)) return string.Empty;
    
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>()
    {
        new Claim("Email", administrador.Email),
        new Claim("Perfil", administrador.Perfil),
        new Claim(ClaimTypes.Role, administrador.Perfil)
    };
    
    var token = new JwtSecurityToken(
        claims: claims,
        expires:  DateTime.Now.AddHours(1),
        signingCredentials: credentials
        );
    
    return new JwtSecurityTokenHandler().WriteToken(token);
}   

ValidationError ValidationAdministradoresDTO(AdministradorDTO administradorDTO)
{
    var validation = new ValidationError
    {
        Mensagens = new List<string>()
    };
    
    if(string.IsNullOrEmpty(administradorDTO.Email)) validation.Mensagens.Add("O Email não pode ser em branco");
    if(string.IsNullOrEmpty(administradorDTO.Senha)) validation.Mensagens.Add("A Senha não pode ser em branco");
    if(administradorDTO.Perfil == null) validation.Mensagens.Add("O Perfil não pode ser em branco");
    
    return validation;
}

app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) =>
{
    var admin = administradorServico.Login(loginDTO);
    if (admin != null)
    {
        string token = GenerateTokenJwt(admin);
        return Results.Ok(new AdminLogado
        {
            Email =  admin.Email,
            Perfil = admin.Perfil,
            Token = token
        });
    }
    return Results.Unauthorized();
}).AllowAnonymous().WithTags("Administradores");

app.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) =>
{
    var validation = ValidationAdministradoresDTO(administradorDTO);
    if(validation.Mensagens.Count > 0) return Results.BadRequest(validation);
    
    var administrador = new Administrador
    {
        Email = administradorDTO.Email,
        Senha = administradorDTO.Senha,
        Perfil = administradorDTO.Perfil.ToString()
    };
    administradorServico.Incluir(administrador);

    var admin = new AdministradorModelView
    {
        Id = administrador.Id,
        Email = administrador.Email,
        Perfil = administrador.Perfil ?? nameof(Perfil.Editor)
    };
    
    return Results.Created($"/administradores/{administrador.Id}", admin);
}).RequireAuthorization(new AuthorizeAttribute{ Roles = "Admin"}).WithTags("Administradores");

app.MapGet("/administradores", ([FromQuery] int? pagina, IAdministradorServico administradorServico) =>
{
    var admins = new List<AdministradorModelView>();
    var administradores = administradorServico.Todos(pagina);
    foreach (var admin in administradores)
    {
        admins.Add(new AdministradorModelView
        {
            Id = admin.Id,
            Email = admin.Email,
            Perfil = admin.Perfil ?? nameof(Perfil.Editor)
        });
    }
    
    return Results.Ok(admins);
}).RequireAuthorization(new AuthorizeAttribute{ Roles = "Admin"}).WithTags("Administradores");

app.MapGet("/administradores/{id}", ([FromRoute]int id, IAdministradorServico administradorServico) =>
{
    var administrador = administradorServico.BuscarPorID(id);
    if(administrador == null) return Results.NotFound();
    var admin = new AdministradorModelView
    {
        Id = administrador.Id,
        Email = administrador.Email,
        Perfil = administrador.Perfil ?? nameof(Perfil.Editor)
    };
    return Results.Ok(admin);
}).RequireAuthorization(new AuthorizeAttribute{ Roles = "Admin"}).WithTags("Administradores");
#endregion

#region Veiculos

ValidationError ValidationVeiculosDTO(VeiculoDTO veiculoDTO)
{
    var validation = new ValidationError
    {
        Mensagens = new List<string>()
    };
    
    if(string.IsNullOrEmpty(veiculoDTO.Nome)) validation.Mensagens.Add("O nome não pode ficar em branco");
    if(string.IsNullOrEmpty(veiculoDTO.Marca)) validation.Mensagens.Add("A marca não pode ficar em branco");
    if(veiculoDTO.Ano < 1950) validation.Mensagens.Add("O ano deve ser a partir de 1950");
    return validation;
}

app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{
    var validation = ValidationVeiculosDTO(veiculoDTO);
    if(validation.Mensagens.Count > 0) return Results.BadRequest(validation);
    
    var veiculo = new Veiculo
    {
        Nome = veiculoDTO.Nome,
        Marca =  veiculoDTO.Marca,
        Ano =  veiculoDTO.Ano
    };
    veiculoServico.Incluir(veiculo);
    
    return Results.Created($"/veiculos/{veiculo.Id}", veiculo);
}).RequireAuthorization(new AuthorizeAttribute{ Roles = "Admin,Editor"}).WithTags("Veiculos");

app.MapGet("/veiculos", ([FromQuery]int? pagina, IVeiculoServico veiculoServico) =>
{
    var veiculos = veiculoServico.Todos(pagina);
    return Results.Ok(veiculos);
}).RequireAuthorization(new AuthorizeAttribute{ Roles = "Admin,Editor"}).WithTags("Veiculos");

app.MapGet("/veiculos/{id}", ([FromRoute]int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);
    return veiculo == null ? Results.NotFound() : Results.Ok(veiculo);
}).RequireAuthorization(new AuthorizeAttribute{ Roles = "Admin,Editor"}).WithTags("Veiculos");

app.MapPut("/veiculos/{id}", ([FromRoute]int id, VeiculoDTO veiculoDTO,IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);
    if (veiculo == null) return Results.NotFound();
    
    var validation = ValidationVeiculosDTO(veiculoDTO);
    if(validation.Mensagens.Count > 0) return Results.BadRequest(validation);

    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = veiculoDTO.Ano;
    
    veiculoServico.Atualizar(veiculo);
    
    return Results.Ok(veiculo);
}).RequireAuthorization(new AuthorizeAttribute{ Roles = "Admin"}).WithTags("Veiculos");

app.MapDelete("/veiculos/{id}", ([FromRoute]int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);
    if (veiculo == null) return Results.NotFound();

    veiculoServico.Excluir(veiculo);
    return Results.NoContent();
}).RequireAuthorization(new AuthorizeAttribute{ Roles = "Admin"}).WithTags("Veiculos");
#endregion

#region App
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.Run();
#endregion


