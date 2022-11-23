using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MIFCore.Hangfire.APIETL;

namespace MAD.DataWarehouse.Xero.XPM.Database.Configurations
{
    internal class ApiDataEntityTypeConfiguration : IEntityTypeConfiguration<ApiData>
    {
        public void Configure(EntityTypeBuilder<ApiData> builder)
        {
            builder.HasKey(y => y.Id);

            builder.Property(y => y.Endpoint).IsRequired().HasMaxLength(450);
            builder.Property(y => y.Uri).IsRequired().HasMaxLength(800);

            builder.HasIndex(y => y.Endpoint);
            builder.HasIndex(y => y.Uri);

            builder
                .Property(y => y.CreatedAt)
                .HasDefaultValueSql("SYSDATETIMEOFFSET()");

            builder
                .HasOne<ApiData>()
                .WithMany()
                .HasForeignKey(y => y.ParentId);
        }
    }
}
