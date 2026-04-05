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

            // ربط الإعجاب بالمستخدم
            builder.HasOne(al => al.User)
                   .WithMany() // يمكن للمستخدم أن يملك إعجابات كثيرة
                   .HasForeignKey(al => al.UserId)
                   .OnDelete(DeleteBehavior.Cascade); // إذا مُسح المستخدم، تُميَح إعجاباته

            // ربط الإعجاب بالمزاد
            builder.HasOne(al => al.Auction)
                   .WithMany() // المزاد يملك إعجابات كثيرة
                   .HasForeignKey(al => al.AuctionId)
                   .OnDelete(DeleteBehavior.NoAction); // لا نمسح المزاد إذا مُسح الإعجاب
        }
    }
}
