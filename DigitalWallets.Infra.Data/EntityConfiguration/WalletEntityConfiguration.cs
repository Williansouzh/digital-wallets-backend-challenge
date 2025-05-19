using DigitalWallets.Domain.Entities;
using DigitalWallets.Infra.Data.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class WalletEntityConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> entity)
    {
        entity.ToTable("Wallets");

        entity.HasKey(w => w.Id);

        entity.Property(w => w.Id)
              .IsRequired()
              .ValueGeneratedNever();

        entity.Property(w => w.Balance)
              .HasColumnType("decimal(18,2)")
              .IsRequired();

        entity.Property(w => w.UserId)
              .IsRequired();

        entity.HasOne<ApplicationUser>() // Referência sem navegação
              .WithOne(u => u.Wallet)
              .HasForeignKey<Wallet>(w => w.UserId)
              .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(w => w.UserId).IsUnique();
    }
}
