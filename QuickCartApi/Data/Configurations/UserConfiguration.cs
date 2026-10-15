using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickCartApi.Entities;

namespace QuickCartApi.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasIndex(u => u.Username)
                .IsUnique();

            builder.Property(u => u.Username)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(u => u.FullName)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(u => u.Email)
                .HasMaxLength(255)
                .IsRequired();

            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.Phone)
                .HasMaxLength(20);

            builder.Property(u => u.Role)
                .HasMaxLength(20)
                .IsRequired();
        }
    }
}
