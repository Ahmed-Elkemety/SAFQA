using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Models;

namespace SAFQA.DAL.Configration
{
    public class ItemConfigration : IEntityTypeConfiguration<Item>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Item> builder)
        {
            builder
                .HasOne(a => a.Auction)
                .WithMany(e => e.items)
                .HasForeignKey(c => c.AuctionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(a => a.images)
                .WithOne(e => e.item)
                .HasForeignKey(c => c.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(a => a.itemAttributesValues)
                .WithOne(e => e.Item)
                .HasForeignKey(c => c.ItemId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Property(i => i.title).IsRequired().HasMaxLength(200);
            builder.Property(i => i.Description).HasMaxLength(2000);
            builder.Property(i => i.Condition).IsRequired();
            builder.Property(i => i.WarrantyInfo).IsRequired().HasMaxLength(500);
            builder.Property(i => i.AuctionId).IsRequired();
            builder.Property(i => i.CategoryId).IsRequired();
        }
    }
}
