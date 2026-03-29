using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MiniMercadoInteligente.Infrastructure.Data;

namespace MiniMercadoInteligente.Infrastructure;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=minimercado;Username=postgres;Password=postgres");

        return new AppDbContext(optionsBuilder.Options);
    }
}