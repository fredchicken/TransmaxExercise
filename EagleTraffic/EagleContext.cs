using Microsoft.EntityFrameworkCore;
using EagleTraffic.Models;

namespace EagleTraffic
{
    public class EagleContext: DbContext
    {
        public DbSet<EagleBot> EagleBots { get; set; }
        //protected readonly IConfiguration Configuration;
        public EagleContext(DbContextOptions<EagleContext> contextOption): base(contextOption)
        {
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder options)
        //{
        //    // connect to sql server with connection string from app settings
        //    options.UseSqlServer(Configuration.GetConnectionString("SqlDatabase"));
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EagleBot>()
                .Property(eb => eb.Id)
                .ValueGeneratedNever();
        }
    }
}
