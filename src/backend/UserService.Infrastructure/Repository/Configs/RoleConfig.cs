using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UserService.Infrastructure.Repository.Configs;

public class RoleConfig : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole
            {
                Name = "Guest",
                NormalizedName = "GUEST",
            },
            new IdentityRole
            {
                Name = "Gambler",
                NormalizedName = "GAMBLER",
            },
            new IdentityRole
            {
                Name = "Moderator",
                NormalizedName = "MODERATOR",
            },
            new IdentityRole
            {
                Name = "Bookmaker",
                NormalizedName = "BOOKMAKER",
            },
            new IdentityRole
            {
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR",
            },
            new IdentityRole
            {
                Name = "Banned",
                NormalizedName = "BANNED",
            }
        );
    }
}