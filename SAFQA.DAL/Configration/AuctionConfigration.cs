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
                   .HasMaxLength(200);

            builder.Property(a => a.Description)
                   .HasMaxLength(3000);

            builder.Property(a => a.StartingPrice)
                   .HasColumnType("decimal(18,2)");

            builder.Property(a => a.CurrentPrice)
                   .HasColumnType("decimal(18,2)");

            builder.Property(a => a.FinalPrice)
                   .HasColumnType("decimal(18,2)");

            builder.Property(a => a.SecurityDeposit).HasDefaultValue(100.50m)
                   .HasColumnType("decimal(18,2)");

            builder.Property(a => a.BidIncrement)
                   .IsRequired();

            // 📌 Defaults
            builder.Property(a => a.CreatedAt)
                   .HasDefaultValueSql("GETDATE()");

            builder.Property(a => a.Status)
                   .HasDefaultValue(AuctionStatus.Upcoming);

            builder.Property(a => a.IsFeatured)
                   .HasDefaultValue(false);

            builder.Property(a => a.IsTrending)
                   .HasDefaultValue(false);

            builder.Property(a => a.HotScore)
                   .HasDefaultValue(false);

            builder.Property(a => a.IsDeleted)
                   .HasDefaultValue(false);
            builder.Property(a => a.WinnerUserId)
                   .HasDefaultValue(" ");

            builder.Property(a=> a.LikesCount)
                   .HasDefaultValue(0);
            builder.Property(a => a.ParticipationCount)
                .HasDefaultValue(0);
            builder.Property(a => a.ViewsCount)
                .HasDefaultValue(0);
            builder.Property(a => a.TotalBids)
                .HasDefaultValue(0);
            

        }
    }
}
