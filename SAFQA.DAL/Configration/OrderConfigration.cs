using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SAFQA.DAL.Migrations;
using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Configration
{
    public class OrderConfigration : IEntityTypeConfiguration<OrderTracking>
    {
        public void Configure(EntityTypeBuilder<OrderTracking> builder)
        {
            builder
                .HasOne(ot => ot.Auction)
                .WithOne(a => a.orderTracking)
                .HasForeignKey<OrderTracking>(ot => ot.AuctionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(e => e.Date)
              .HasDefaultValueSql("GETDATE()")
              .IsRequired();

            builder.Property(e => e.IsCompleted)
              .HasDefaultValue(false);

            builder.HasIndex(e => e.AuctionId);
        }
    }
}