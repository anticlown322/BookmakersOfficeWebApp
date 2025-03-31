using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Models;

namespace UserService.Infrastructure.Repository.Configs;

public class BalanceTransactionConfiguration : IEntityTypeConfiguration<BalanceTransaction>
{
    public void Configure(EntityTypeBuilder<BalanceTransaction> builder)
    {
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(bt => bt.Amount)
            .HasColumnType("decimal(18, 2)")
            .IsRequired();

        builder.Property(bt => bt.CreatedAt)
            .IsRequired();

        builder.Property(bt => bt.OperationType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(bt => bt.Comment)
            .HasMaxLength(255);

        builder.HasOne(bt => bt.User)
            .WithMany()
            .HasForeignKey(bt => bt.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(up => up.UserId);

        builder.Ignore(ub => ub.User);
    }
}