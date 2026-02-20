using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Models;

namespace SAFQA.DAL.Configration
{
    public class SellerConfigration : IEntityTypeConfiguration<Seller>
    {
        
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Seller> builder)
        {

            builder
                .HasOne(a => a.User)
                .WithOne(e => e.Seller)
                .HasForeignKey<Seller>(a => a.UserId)
                .IsRequired(false);

            builder
                .HasOne(c => c.City)
                .WithMany(u => u.sellers)
                .HasForeignKey(c => c.CityId)
                .IsRequired(false);

            // Seller 1 - 1 PersonalSeller
            builder.HasOne(s => s.PersonalSeller)
                .WithOne(p => p.Seller)
                .HasForeignKey<PersonalSeller>(p => p.SellerId);

            // Seller 1 - 1 BusinessSeller
            builder.HasOne(s => s.BusinessSeller)
                .WithOne(b => b.Seller)
                .HasForeignKey<BusinessSeller>(b => b.SellerId);


            builder.Property(s => s.StoreName).IsRequired().HasMaxLength(150);
            builder.Property(s => s.PhoneNumber).IsRequired().HasMaxLength(20);
            builder.Property(s => s.Description).HasMaxLength(1000);
            builder.Property(s => s.Rating).HasDefaultValue(0);
            builder.Property(s => s.StoreLogo).HasColumnType("varbinary(max)");
            builder.Property(s => s.SellerAt).HasDefaultValueSql("GETDATE()");
            builder.Property(s => s.BussinessType).IsRequired();
            builder.Property(s => s.VerificationStatus).IsRequired();
            builder.Property(s => s.StoreStatus).IsRequired();
            builder.Property(s => s.DeletedAt).HasDefaultValueSql("GETDATE()");
        }
    }
}
