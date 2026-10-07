using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickCartApi.Entities;

namespace QuickCartApi.Data.Configurations
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.Property(a => a.RecipientName).HasMaxLength(150).IsRequired();
            builder.Property(a => a.ContactNumber).HasMaxLength(20).IsRequired();
            builder.Property(a => a.AddressLine).HasMaxLength(255).IsRequired();
            builder.Property(a => a.Barangay).HasMaxLength(100).IsRequired();
            builder.Property(a => a.City).HasMaxLength(100).IsRequired();
            builder.Property(a => a.Province).HasMaxLength(100).IsRequired();
            builder.Property(a => a.PostalCode).HasMaxLength(10).IsRequired();

            // user -> address
            builder.HasOne(a => a.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(a => new { a.UserId, a.IsDefault })
                .HasFilter("[IsDefault] = 1")
                .IsUnique();
        }
    }
}
