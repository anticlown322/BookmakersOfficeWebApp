using BettingService.DAL.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BettingService.DAL.Repositories.Configs;

public class BetConfig : IEntityTypeConfiguration<Bet>
{
    public void Configure(EntityTypeBuilder<Bet> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(b => b.Username)
            .IsRequired();

        builder.Property(b => b.MatchId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(b => b.Amount)
            .IsRequired()
            .HasColumnType("decimal(18, 2)");

        builder.Property(b => b.Odds)
            .IsRequired()
            .HasColumnType("decimal(8, 2)");

        builder.Property(b => b.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(b => b.CreatedAt)
            .IsRequired();

        builder.Property(b => b.SettledAt)
            .IsRequired(false);

        builder.HasIndex(b => b.Username);
        builder.HasIndex(b => b.MatchId);
    }
}