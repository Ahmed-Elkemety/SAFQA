using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Models;

namespace SAFQA.DAL.Configration
{
    public class ReviewConfigration : IEntityTypeConfiguration<Review>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Review> builder)
        {
            builder
                .HasOne(a => a.User)
                .WithMany(e => e.reviews)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(a => a.Seller)
                .WithMany(e => e.reviews)
                .HasForeignKey(p => p.SellerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(r => r.Rating).IsRequired();
            builder.Property(r => r.Comment).HasMaxLength(500);
            builder.Property(r => r.Comment).HasMaxLength(500);
            builder.Property(r => r.Date).HasDefaultValueSql("GETDATE()").IsRequired();
            builder.HasIndex(r => r.UserId);
            builder.HasIndex(r => r.SellerId);
            builder.HasIndex(r => r.Rating);
            builder.HasIndex(r => r.Date);
        }
    }
}
