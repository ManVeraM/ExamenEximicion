using ExamenEximicion.RegistroInaiso.Models;
using Microsoft.EntityFrameworkCore;

namespace ExamenEximicion.Libreria.Models;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }

    public DbSet<App> Apps{ get; set; } = null!;

    public DbSet<User> Users { get; set; } = null!;

    public DbSet<Reservation> Reservations { get; set; } = null!;
    
}