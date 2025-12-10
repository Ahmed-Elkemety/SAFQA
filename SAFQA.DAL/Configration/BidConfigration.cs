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
                .HasForeignKey(a => a.UserId)
                .IsRequired(false);

            builder
                .HasOne(e => e.Auction)
                .WithMany(a => a.Bids)
                .HasForeignKey(e => e.AuctionId)
                .IsRequired(false);

            builder
                .HasOne(e => e.proxyBidding)
                .WithMany(a => a.bids)
                .HasForeignKey(e => e.ProxyBiddingId)
                .IsRequired(false);


            builder.Property(b => b.Type).IsRequired();
            builder.Property(b => b.Date).IsRequired();
            builder.Property(c => c.Amount).IsRequired();

        }
    }
}
