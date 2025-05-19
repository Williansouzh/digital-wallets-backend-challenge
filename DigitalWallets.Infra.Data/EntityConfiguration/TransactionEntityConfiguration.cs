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

        entity.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(255);

        entity.Property(t => t.Timestamp)
            .IsRequired();

        entity.Property(t => t.Status)
            .IsRequired()
            .HasConversion<string>();

        entity.Property(t => t.Type)
            .IsRequired()
            .HasConversion<string>();

        entity.Property(t => t.SenderId)
            .IsRequired(false); // Optional for credit transactions

        entity.Property(t => t.RecipientId)
            .IsRequired(false); // Optional for debit transactions

        // Indexes for query performance
        entity.HasIndex(t => t.SenderId);
        entity.HasIndex(t => t.RecipientId);
        entity.HasIndex(t => t.Timestamp);
        entity.HasIndex(t => t.Status);
        entity.HasIndex(t => t.Type);
    }
}