using Microsoft.EntityFrameworkCore;
using MIFCore.Hangfire.APIETL.Extract;

namespace MAD.DataWarehouse.Xero.XPM.Database
{
    internal class XeroDbContext : DbContext
    {
        public XeroDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ApiData> ApiData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(XeroDbContext).Assembly);
        }
    }
}
