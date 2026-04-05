using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Models;

namespace SAFQA.DAL.Configration
{
    public class AuctionViewConfiguration : IEntityTypeConfiguration<AuctionView>
    {
        public void Configure(EntityTypeBuilder<AuctionView> builder)
        {
            builder.ToTable("AuctionViews");

            builder.HasKey(av => av.Id);

            // ربط المشاهدة بالمستخدم
            builder.HasOne(av => av.User)
                   .WithMany()
                   .HasForeignKey(av => av.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // ربط المشاهدة بالمزاد
            builder.HasOne(av => av.Auction)
                   .WithMany()
                   .HasForeignKey(av => av.AuctionId)
                   .OnDelete(DeleteBehavior.NoAction);

            // إعدادات إضافية
            builder.Property(av => av.ViewedAt)
                   .HasDefaultValueSql("GETDATE()"); // جعل الوقت تلقائي من السيرفر

            builder.Property(av => av.DeviceType)
                   .HasMaxLength(100);
        }
    }
}
