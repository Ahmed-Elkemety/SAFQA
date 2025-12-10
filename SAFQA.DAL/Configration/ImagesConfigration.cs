using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Models;

namespace SAFQA.DAL.Configration
{
    public class ImagesConfigration : IEntityTypeConfiguration<Images>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Images> builder)
        {   
            builder
                .HasOne(img => img.item)
                .WithMany(i => i.images)
                .HasForeignKey(img => img.ItemId);

            builder.Property(img => img.Image).IsRequired();
            builder.Property(img => img.IsMain).HasDefaultValue(false);
            builder.Property(img => img.ItemId).IsRequired();
        }
    }
}
