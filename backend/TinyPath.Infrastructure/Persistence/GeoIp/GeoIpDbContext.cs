using System.Net;
using Microsoft.EntityFrameworkCore;
using TinyPath.Application.Interfaces;
using TinyPath.Domain.Entities.GeoIp;

namespace TinyPath.Infrastructure.Persistence.GeoIp;

public class GeoIpDbContext : DbContext, IGeoIpDbContext
{
    public DbSet<Country> Country { get; set; }
    
    public GeoIpDbContext(DbContextOptions<GeoIpDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Country>().HasNoKey();
    }
}