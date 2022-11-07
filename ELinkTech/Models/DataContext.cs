using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
//using System.Data.Entity;

namespace ELinkTech.Models
{
    public class DataContext:IdentityDbContext<ApplicationUser>
    {
        public DbSet<Category> categories { get; set; }

        public DbSet<Order> orders { get; set; }

        public DbSet<Product> products { get; set; }

        public DbSet<Supplier> suppliers { get; set; }

        public DbSet<Quote> quotes { get; set; }

        public DbSet<ApplicationUser> users { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }
    }
}
