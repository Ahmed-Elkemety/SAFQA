using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SAFQA.BLL.Enums;
using SAFQA.DAL.Models;

namespace SAFQA.DAL.Configration
{
    public class AuctionConfigration : IEntityTypeConfiguration<Auction>
    {
        public void Configure(EntityTypeBuilder<Auction> builder)
        {
            // 🔗 Seller Relation
            builder.HasOne(a => a.Seller)
                   .WithMany(s => s.Auctions)
                   .HasForeignKey(a => a.SellerId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(false);

            // 🔗 One-to-One Delivery
            builder.HasOne(a => a.delivery)
                   .WithOne(d => d.Auction)
                   .HasForeignKey<Delivery>(d => d.AuctionId)
                   .OnDelete(DeleteBehavior.Cascade);

            // 🔗 One-to-One Review
            builder.HasOne(a => a.review)
                   .WithOne(r => r.auction)
                   .HasForeignKey<Review>(r => r.AuctionId)
                   .OnDelete(DeleteBehavior.Cascade);

            // 📌 Required Fields
            builder.Property(a => a.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(a => a.Description)
                   .IsRequired()
                   .HasMaxLength(3000);

            builder.Property(a => a.StartingPrice)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(a => a.CurrentPrice)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(a => a.FinalPrice)
                   .HasColumnType("decimal(18,2)").HasDefaultValue(100.50m)
                   .IsRequired(true);

            builder.Property(a => a.SecurityDeposit).HasDefaultValue(100.50m)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired(true);

            builder.Property(a => a.BidIncrement)
                   .IsRequired();

            // 📌 Defaults
            builder.Property(a => a.CreatedAt)
                   .HasDefaultValueSql("GETDATE()");

            builder.Property(a => a.UpdatedAt)
                   .HasDefaultValueSql("GETDATE()");

            builder.Property(a => a.Status)
                   .HasDefaultValue(AuctionStatus.Upcoming)
                   .IsRequired();

            builder.Property(a => a.IsFeatured)
                   .HasDefaultValue(false);

            builder.Property(a => a.IsTrending)
                   .HasDefaultValue(false);

            builder.Property(a => a.HotScore)
                   .HasDefaultValue(false);

            builder.Property(a => a.IsDeleted)
                   .HasDefaultValue(false)
                   .IsRequired();

            builder.Property(a => a.DeletedAt)
                   .IsRequired(false);

            // 📌 Optional fields
            builder.Property(a => a.WinnerUserId).HasDefaultValue(" ")
                   .IsRequired(true);

            builder.Property(a => a.Image)
                   .IsRequired(false);
        }
    }
}
