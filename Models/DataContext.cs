
using Microsoft.EntityFrameworkCore;

namespace inmobiliaria_mvc.Models;

public class DataContext : DbContext
{
    
    public DataContext(DbContextOptions options) : base(options)
    {
    }
    
    public DbSet<Inquilino> Inquilinos { get; set; }

}