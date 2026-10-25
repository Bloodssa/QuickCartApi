using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickCartApi.Entities;

namespace QuickCartApi.Data.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.Property(t => t.TransactionReference).HasMaxLength(50).IsRequired();
            builder.Property(t => t.PaymentMethod).HasMaxLength(20).IsRequired();
            builder.Property(t => t.PaymentStatus).HasMaxLength(20).IsRequired();
            builder.Property(t => t.Amount).HasPrecision(10, 2);

            // order -> transaction
            builder.HasIndex(t => t.OrderId).IsUnique();

            builder.HasOne(t => t.Order)
                .WithOne(o => o.Transaction)
                .HasForeignKey<Transaction>(t => t.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(t => t.PaymentStatus).HasConversion<string>().HasMaxLength(20).IsRequired();
        }
    }
}
