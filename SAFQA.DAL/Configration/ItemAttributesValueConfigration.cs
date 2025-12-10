using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Models;

namespace SAFQA.DAL.Configration
{
    public class ItemAttributesValueConfigration : IEntityTypeConfiguration<ItemAttributesValue>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ItemAttributesValue> builder)
        {
            builder
                .HasOne(a => a.categoryAttributes)
                .WithMany(e => e.itemAttributesValues)
                .HasForeignKey(a => a.CategoryAttributeId);
            builder
                .HasOne(i => i.Item)
                .WithMany(e => e.itemAttributesValues)
                .HasForeignKey(i => i.ItemId)
                .IsRequired(false);
        }
    }
}
