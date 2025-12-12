using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Models;

namespace SAFQA.DAL.Configration
{
    public class DeliveryConfigration : IEntityTypeConfiguration<Delivery>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Delivery> builder)
        {
            builder.HasOne(a => a.Auction)
                   .WithOne(e => e.delivery)
                   .HasForeignKey<Delivery>(a => a.AuctionId);

            builder
                .HasOne(a => a.User)
                .WithMany(e => e.Deliveries)
                .HasForeignKey(a => a.UserId)
                .IsRequired(false);

            builder
                .HasOne(a => a.Seller)
                .WithMany(e => e.deliveries)
                .HasForeignKey(a => a.SellerId)
                .IsRequired(false);
            builder.Property(d => d.Code)
              .IsRequired()
              .HasMaxLength(50);

            
            builder.Property(d => d.ComfirmedAt).IsRequired();
            builder.Property(d => d.ContactNumber).IsRequired().HasMaxLength(20);
            builder.Property(d => d.ProfImage).HasColumnType("varbinary(max)").IsRequired();
            builder.Property(d => d.Status).IsRequired().HasConversion<int>();
        }
    }
}
