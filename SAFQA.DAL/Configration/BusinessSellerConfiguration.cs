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
    public class BusinessSellerConfiguration : IEntityTypeConfiguration<BusinessSeller>
    {
        public void Configure(EntityTypeBuilder<BusinessSeller> builder)
        {
            builder.ToTable("BusinessSellers");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.BankName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(b => b.IBAN)
                .IsRequired()
                .HasMaxLength(34);

            builder.Property(b => b.CommercialRegister)
                .IsRequired();

            builder.Property(b => b.TaxId)
                .IsRequired();

            builder.HasIndex(b => b.SellerId)
                .IsUnique();
        }
    }
}
