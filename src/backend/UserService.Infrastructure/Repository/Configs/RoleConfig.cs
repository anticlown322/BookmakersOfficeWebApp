using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Infrastructure.Utility;

namespace UserService.Infrastructure.Repository.Configs;

public class RoleConfig : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole { Name = UserRoles.Guest, NormalizedName = UserRoles.NormalizedGuest },
            new IdentityRole { Name = UserRoles.Gambler, NormalizedName = UserRoles.NormalizedGambler },
            new IdentityRole { Name = UserRoles.Moderator, NormalizedName = UserRoles.NormalizedModerator },
            new IdentityRole { Name = UserRoles.Bookmaker, NormalizedName = UserRoles.NormalizedBookmaker },
            new IdentityRole { Name = UserRoles.Administrator, NormalizedName = UserRoles.NormalizedAdministrator },
            new IdentityRole { Name = UserRoles.Banned, NormalizedName = UserRoles.NormalizedBanned });
    }
}