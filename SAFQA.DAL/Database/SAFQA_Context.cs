using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Configration;
using SAFQA.DAL.Models;

namespace SAFQA.DAL.Database
{
    public class SAFQA_Context:IdentityDbContext<User>
    {
        public readonly object RefreshTokens;

        public SAFQA_Context(DbContextOptions<SAFQA_Context>options):base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new AuctionConfigration());
            modelBuilder.ApplyConfiguration(new BidConfigration());
            modelBuilder.ApplyConfiguration(new CategoryAttributesConfigration());
            modelBuilder.ApplyConfiguration(new CategoryConfigration());
            modelBuilder.ApplyConfiguration(new CityConfigration());
            modelBuilder.ApplyConfiguration(new CountryConfigration());
            modelBuilder.ApplyConfiguration(new DeliveryConfigration());
            modelBuilder.ApplyConfiguration(new DisputesConfigration());
            modelBuilder.ApplyConfiguration(new ImagesConfigration());
            modelBuilder.ApplyConfiguration(new ItemAttributesValueConfigration());
            modelBuilder.ApplyConfiguration(new ItemConfigration());
            modelBuilder.ApplyConfiguration(new NotificationConfigration());
            modelBuilder.ApplyConfiguration(new ProxyBiddingConfigration());
            modelBuilder.ApplyConfiguration(new ReviewConfigration());
            modelBuilder.ApplyConfiguration(new SellerConfigration());
            modelBuilder.ApplyConfiguration(new TransactionConfigration());
            modelBuilder.ApplyConfiguration(new WalletConfigration());
            modelBuilder.ApplyConfiguration(new RefreshTokenConfig());

            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                    .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Seller> Sellers { get; set; }
        public DbSet<Auction> Auctions { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Delivery> Delivery { get; set; }
        public DbSet<Disputes> Disputes { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ProxyBidding> proxyBiddings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Transactions> Transactions { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Country> countries { get; set; }
        public DbSet<City> cities { get; set; }
        public DbSet<Images> images { get; set; }
        public DbSet<CategoryAttributes> categoryAttributes { get; set; }
        public DbSet<ItemAttributesValue> itemAttributesValues { get; set; }
        public DbSet<RefreshToken> refreshTokens { get; set; }
    }
}
