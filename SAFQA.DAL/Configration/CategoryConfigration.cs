using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Models;

namespace SAFQA.DAL.Configration
{
    public class CategoryConfigration : IEntityTypeConfiguration<Category>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Category> builder)
        {
            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.HasIndex(c => c.Name).IsUnique();
            builder.Property(c => c.Description).HasMaxLength(500);
            builder.HasIndex(c => c.Name);
            builder.HasIndex(c => c.Id);
        }
    }
}
