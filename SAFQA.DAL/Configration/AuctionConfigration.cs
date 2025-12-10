using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Models;

namespace SAFQA.DAL.Configration
{
    public class AuctionConfigration : IEntityTypeConfiguration<Auction>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Auction> builder)
        {
            builder.HasOne(a => a.Seller)
                   .WithMany(e => e.Auctions)
                   .HasForeignKey(a => a.SellerId)
                   .IsRequired(false);

            builder.Property(a => a.Title).IsRequired().HasMaxLength(200);
            builder.Property(a => a.Description).IsRequired().HasMaxLength(3000);
            builder.Property(a => a.CreatedAt).HasDefaultValueSql("GETDATE()");
            builder.Property(a => a.UpdatedAt).HasDefaultValueSql("GETDATE()");
            builder.Property(a => a.Status).IsRequired();
            builder.Property(a => a.IsFeatured).HasDefaultValue(false);
            builder.Property(a => a.IsTrending).HasDefaultValue(false);
            builder.Property(a => a.HotScore).HasDefaultValue(false);
        }
    }
}
