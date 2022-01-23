using Microsoft.EntityFrameworkCore;
using UsedAPI.Models;

namespace UsedAPI.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    
    public DbSet<Item> Items { get; set; }

    public DbSet<Seller> Sellers { get; set; }
}