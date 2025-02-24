using DynamicSun.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace DynamicSun.Data;

public class WeatherDbContext : DbContext
{
    public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options) { }
    public DbSet<Weather> Weather { get; set; }
    
}