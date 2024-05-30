using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PoliceApp.Models;

public class PoliceDbContext : IdentityDbContext
{
    public PoliceDbContext(DbContextOptions<PoliceDbContext> options)
        : base(options)
    {
    }

    public DbSet<Caz> Cazuri { get; set; }
    public DbSet<Utilizator> Utilizatori { get; set; }
    public DbSet<Angajat> Angajati { get; set; }
    public DbSet<Raport> Rapoarte { get; set; }
    public DbSet<Comentariu> Comentarii { get; set; }
}
