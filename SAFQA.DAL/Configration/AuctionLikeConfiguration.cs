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
    public class AuctionLikeConfiguration : IEntityTypeConfiguration<AuctionLike>
    {
        public void Configure(EntityTypeBuilder<AuctionLike> builder)
        {
            builder.ToTable("AuctionLikes");

            builder.HasKey(al => al.Id);

            builder.HasOne(al => al.User)
                   .WithMany() 
                   .HasForeignKey(al => al.UserId)
                   .OnDelete(DeleteBehavior.Cascade); 

            builder.HasOne(al => al.Auction)
                   .WithMany() 
                   .HasForeignKey(al => al.AuctionId)
                   .OnDelete(DeleteBehavior.NoAction); 
        }
    }
}
