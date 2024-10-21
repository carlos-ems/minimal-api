using Microsoft.EntityFrameworkCore;
using minimal_api.Entity;

namespace minimal_api.Databases;

public class DatabaseContext : DbContext
{
    private readonly IConfiguration _configureAppSettings;

    public DatabaseContext(IConfiguration configureAppSettings)
    {
        _configureAppSettings = configureAppSettings;
    }

    public DbSet<Admin> Admins { get; set; } = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(!optionsBuilder.IsConfigured)
        {
            var connectionString = _configureAppSettings.GetConnectionString("mysql")?.ToString();

            if(!string.IsNullOrEmpty(connectionString))
            {
                optionsBuilder.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString)
                ); 
            }
        }
    }
}
