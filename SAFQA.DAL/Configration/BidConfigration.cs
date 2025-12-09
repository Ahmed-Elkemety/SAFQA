using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Models;

namespace SAFQA.DAL.Configration
{
    public class BidConfigration : IEntityTypeConfiguration<Bid>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Bid> builder) 
        {
            builder
                .HasOne(a => a.User)
                .WithMany(e => e.Bids)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder
                .HasOne(e => e.Auction)
                .WithMany(a => a.Bids)
                .HasForeignKey(a => a.AuctionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(r => r.Notifications)
                .WithOne(e => e.Bid)
                .HasForeignKey(b => b.ReferenceId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Property(b => b.Type).IsRequired();
            builder.Property(b => b.Date).IsRequired();
            builder.Property(c => c.Amount).HasColumnType("decimal(18,2)").IsRequired();
            builder.HasIndex(b => new { b.AuctionId, b.BidOrder }).IsUnique();

        }
    }
}
