using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WalletSystem.Models.Entity;

namespace WalletSystem.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<TotalBalance> TotalBalances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Wallet>()
                .HasOne(w => w.User)
                .WithMany(u => u.Wallets)
                .HasForeignKey(w => w.UserId);

            modelBuilder.Entity<TotalBalance>()
           .Property(u => u.Balance)
           .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Wallet>()
            .Property(w => w.AccountBalance)
            .HasColumnType("decimal(18,2)");
        }
         
    }


}
