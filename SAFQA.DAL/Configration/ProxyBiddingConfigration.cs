using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Models;

namespace SAFQA.DAL.Configration
{
    public class ProxyBiddingConfigration : IEntityTypeConfiguration<ProxyBidding>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ProxyBidding> builder)
        {
            builder.HasOne(a => a.auction)
                   .WithMany(pb => pb.ProxyBiddings)
                   .HasForeignKey(a => a.AuctionId)
                   .IsRequired(false);

            builder.HasOne(u => u.user)
                    .WithMany(pb => pb.proxyBiddings)
                    .HasForeignKey(u => u.UserId)
                    .IsRequired(false);

            builder.Property(pb => pb.Status).IsRequired();
            builder.Property(pb => pb.Max).IsRequired();
            builder.Property(pb => pb.Step).IsRequired();
            builder.Property(pb => pb.CreatedAt).HasDefaultValueSql("GETDATE()").IsRequired();
            builder.Property(pb => pb.UpdateAt).HasDefaultValueSql("GETDATE()").IsRequired();
        }
    }
}
