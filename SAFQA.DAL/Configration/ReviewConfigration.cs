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
                .IsRequired(false);

            builder
                .HasOne(a => a.Seller)
                .WithMany(e => e.reviews)
                .HasForeignKey(p => p.SellerId)
                .IsRequired(false);

            builder.Property(r => r.Rating).IsRequired();
            builder.Property(r => r.Comment).HasMaxLength(500);
            builder.Property(r => r.Comment).HasMaxLength(500);
            builder.Property(r => r.Date).HasDefaultValueSql("GETDATE()").IsRequired();
        }
    }
}
