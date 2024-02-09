using Microsoft.EntityFrameworkCore;
using TinyPath.Domain.Entities.GeoIp;

namespace TinyPath.Application.Interfaces;

public interface IGeoIpDbContext
{
    DbSet<Country> Country { get; set; }
}