using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
//using System.Data.Entity;

namespace ELinkTech.Models
{
    public class DataContextP : DbContext
    {
        public DbSet<Product> products { get; set; }

        public DataContextP(DbContextOptions<DataContextP> options) : base(options)
        {

        }
    }
}
