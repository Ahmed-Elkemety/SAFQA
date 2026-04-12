using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using Google;
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

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SAFQA_Context).Assembly);



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
        public DbSet<AuctionUser> AuctionUsers { get; set; }
        public DbSet<PasswordResetOtp> PasswordResetOtps { get; set; }
        public DbSet<PendingUserRegistration> PendingUserRegistrations { get; set; }
        public DbSet<SavedCard> savedCards { get; set; }
        public DbSet<PersonalSeller> personalSellers { get; set; }
        public DbSet<BusinessSeller> businessSellers { get; set; }
        public DbSet<AuctionLike> auctionLikes { get; set; }
        public DbSet<AuctionView> auctionViews { get; set; }
        public DbSet<AuctionReport> auctionReports { get; set; }

    }
}
