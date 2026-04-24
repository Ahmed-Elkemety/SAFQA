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
    public class AuctionParticipationsConfiguration : IEntityTypeConfiguration<AuctionParticipations>
    {
        public void Configure(EntityTypeBuilder<AuctionParticipations> builder)
        {
            builder.ToTable("BidParticipations");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.PatoicipationTime)
                   .HasColumnType("datetime2(7)")
                   .HasDefaultValueSql("GETDATE()");

            builder.HasKey(x => new { x.AuctionId, x.UserId });

            builder
                .HasOne(x => x.Auction)
                .WithMany(a => a.auctionParticipations)
                .HasForeignKey(x => x.AuctionId);

            builder
                .HasOne(x => x.User)
                .WithMany(u => u.auctionParticipations)
                .HasForeignKey(x => x.UserId);
        }
    }
}
