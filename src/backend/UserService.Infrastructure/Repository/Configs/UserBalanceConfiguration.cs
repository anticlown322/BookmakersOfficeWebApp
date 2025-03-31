using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Models;

namespace UserService.Infrastructure.Repository.Configs;

public class UserBalanceConfiguration : IEntityTypeConfiguration<UserBalance>
{
    public void Configure(EntityTypeBuilder<UserBalance> builder)
    {
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(ub => ub.CurrentAmount)
            .HasColumnType("decimal(18, 2)")
            .IsRequired();

        builder.Property(ub => ub.LastUpdated)
            .IsRequired();

        builder.HasOne(ub => ub.User)
            .WithMany()
            .HasForeignKey(ub => ub.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(ub => ub.Transactions)
            .WithOne()
            .HasForeignKey(bt => bt.UserId)
            .HasPrincipalKey(ub => ub.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(up => up.UserId)
            .IsUnique();

        builder.Ignore(ub => ub.User);
    }
}