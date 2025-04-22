using BettingService.DAL.Models.Entities;
using BettingService.DAL.Repositories.Configs;
using Microsoft.EntityFrameworkCore;

namespace BettingService.DAL.Repositories;

public class RepositoryContext(
    DbContextOptions<RepositoryContext> options) 
    : DbContext(options)
{
    public DbSet<Bet> Bets { get; set; }
    public DbSet<Payout> Payouts { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BetConfig());
        modelBuilder.ApplyConfiguration(new PayoutConfig());
    }
}