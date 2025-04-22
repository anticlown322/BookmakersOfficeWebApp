using BettingService.DAL.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BettingService.DAL.Repositories.Configs;

public class PayoutConfig : IEntityTypeConfiguration<Payout>
{
    public void Configure(EntityTypeBuilder<Payout> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(p => p.BetId)
            .IsRequired();

        builder.Property(p => p.Amount)
            .IsRequired()
            .HasColumnType("decimal(18, 2)");

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.ProcessedAt)
            .IsRequired(false);

        builder.Property(p => p.ErrorReason)
            .HasMaxLength(500);

        builder.HasOne<Bet>()
            .WithOne()
            .HasForeignKey<Payout>(p => p.BetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}