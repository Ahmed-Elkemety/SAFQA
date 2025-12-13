using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SAFQA.DAL.Models;

namespace SAFQA.DAL.Configration
{
    public class AuctionUserConfiguration : IEntityTypeConfiguration<AuctionUser>
    {
        public void Configure(EntityTypeBuilder<AuctionUser> builder)
        {
            builder.HasKey(x => new { x.AuctionId, x.UserId });

            builder
                .HasOne(x => x.Auction)
                .WithMany(a => a.AuctionUsers)
                .HasForeignKey(x => x.AuctionId);

            builder
                .HasOne(x => x.User)
                .WithMany(u => u.AuctionUsers)
                .HasForeignKey(x => x.UserId);

            builder
                .Property(x => x.JoinedAt)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}
