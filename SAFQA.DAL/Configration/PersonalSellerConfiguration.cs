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
    public class PersonalSellerConfiguration : IEntityTypeConfiguration<PersonalSeller>
    {
        public void Configure(EntityTypeBuilder<PersonalSeller> builder)
        {
            builder.ToTable("PersonalSellers");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.NationalIdFront)
                .IsRequired();

            builder.Property(p => p.NationalIdBack)
                .IsRequired();

            builder.Property(p => p.SelfieWithId)
                .IsRequired();

            builder.HasIndex(p => p.SellerId)
                .IsUnique(); // 1-1 relationship
        }
    }
}
