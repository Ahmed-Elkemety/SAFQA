using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Models;

namespace SAFQA.DAL.Configration
{
    public class DisputesConfigration : IEntityTypeConfiguration<Disputes>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Disputes> builder)
        {

            builder
                .HasOne(a => a.Auction)
                .WithMany(e => e.disputes)
                .HasForeignKey(a => a.AuctionId)
                .IsRequired(false);
            builder
                .HasOne(a => a.Delivery)
                .WithMany(e => e.Disputes)
                .HasForeignKey(a => a.DeliveryId)
                .IsRequired(false);
            builder
                .HasOne(a => a.User)
                .WithMany(e => e.Disputes)
                .HasForeignKey(a => a.UserId)
                .IsRequired(false);


            builder.Property(d => d.Title).IsRequired().HasMaxLength(200);
            builder.Property(d => d.Reason).IsRequired().HasMaxLength(1000);
            builder.Property(d => d.Date).IsRequired().HasDefaultValueSql("GETDATE()");
        }
    }
}
