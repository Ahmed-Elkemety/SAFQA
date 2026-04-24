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
    public class UserFollowersConfiguration : IEntityTypeConfiguration<UserFollowers>
    {
        public void Configure(EntityTypeBuilder<UserFollowers> builder)
        {

            builder.HasKey(f => f.Id);


            builder.Property(f => f.FollowedAt)
                .HasColumnType("datetime2(7)")
                .HasDefaultValueSql("GETDATE()");

            builder.HasKey(x => new { x.SellerId, x.UserId });

            builder
                .HasOne(x => x.Seller)
                .WithMany(a => a.userFollowers)
                .HasForeignKey(x => x.SellerId);

            builder
                .HasOne(x => x.User)
                .WithMany(u => u.UserFollowers)
                .HasForeignKey(x => x.UserId);

            builder.HasIndex(f => new { f.UserId, f.SellerId }).IsUnique();
        }
    }
}
