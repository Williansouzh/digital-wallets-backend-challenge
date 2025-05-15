using DigitalWallets.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalWallets.Infra.Data.EntityConfiguration;

internal class TransactionEntityConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> entity)
    {
        entity.ToTable("Transactions");

        entity.HasKey(t => t.Id);

        entity.Property(t => t.Id)
            .IsRequired()
            .ValueGeneratedNever();

        entity.Property(t => t.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        entity.Property(t => t.Timestamp)
            .IsRequired();

        entity.Property(t => t.SenderId)
            .IsRequired();

        entity.Property(t => t.RecipientId)
            .IsRequired();
    }
}
