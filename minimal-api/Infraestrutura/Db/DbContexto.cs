using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using minimal_api.Dominio.Entidades;

namespace minimal_api.Infraestrutura.Db;

public class DbContexto : DbContext
{
    private readonly IConfiguration _configuration;

    public DbContexto(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public DbSet<Administrador>? Administradores { get; set; } = default;
    public DbSet<Veiculo>? Veiculos { get; set; } = default;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrador>().HasData(
            new Administrador
                {
                    Id = 1,
                    Email = "admin@email.com",
                    Senha = "senha",
                    Perfil = "Admin"
                }
            );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        { 
            string connectionAddress = _configuration.GetConnectionString("mysql")?.ToString();

            if (!string.IsNullOrEmpty(connectionAddress))
            {
                optionsBuilder.UseMySql(connectionAddress, 
                    ServerVersion.AutoDetect(connectionAddress));
            }
        }
       
    }
}