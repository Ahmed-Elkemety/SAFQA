using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Models;

namespace SAFQA.DAL.Configration
{
    public class CategoryAttributesConfigration : IEntityTypeConfiguration<CategoryAttributes>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<CategoryAttributes> builder)
        {
            builder
                .HasOne(ca => ca.category)
                .WithMany(c => c.CategoryAttributes)
                .HasForeignKey(ca => ca.CategoryId)
                .IsRequired(false);

            builder.Property(ca => ca.Name).IsRequired().HasMaxLength(150);
            builder.Property(ca => ca.dataType).IsRequired();
            builder.Property(ca => ca.unit).IsRequired();
            builder.Property(ca => ca.IsRequird).HasDefaultValue(false).IsRequired();
        }
    }
}
