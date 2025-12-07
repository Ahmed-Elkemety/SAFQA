using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Models;

namespace SAFQA.DAL.Database
{
    public class SAFQA_Context:DbContext
    {
        public SAFQA_Context(DbContextOptions<SAFQA_Context>options):base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ProxyBidding>()
                    .HasKey(pb => new { pb.ProxyId, pb.BidId });

            foreach (var fk in modelBuilder.Model.GetEntityTypes()
                    .SelectMany(e => e.GetForeignKeys()))
            {
                fk.DeleteBehavior = DeleteBehavior.NoAction;
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
    }
}
