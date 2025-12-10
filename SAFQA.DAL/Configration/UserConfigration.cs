using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Models;

namespace SAFQA.DAL.Configration
{
    public class UserConfigration : IEntityTypeConfiguration<User>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<User> builder)
        {
            builder
                .HasOne(c => c.City)
                .WithMany(u => u.users)
                .HasForeignKey(c => c.CityId)
                .IsRequired(false);



            builder.Property(u => u.FullName).IsRequired().HasMaxLength(150);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(150);
            builder.HasIndex(u => u.Email).IsUnique();
            builder.Property(u => u.Password).IsRequired().HasMaxLength(500); 
            builder.Property(u => u.PhoneNumber).HasMaxLength(20);
            builder.Property(u => u.Gender).HasMaxLength(10);
            builder.Property(u => u.CreatedAt).HasDefaultValueSql("GETDATE()");
            builder.Property(u => u.UpdatedAt).HasDefaultValueSql("GETDATE()");
            builder.Property(u => u.LastLogin).HasDefaultValueSql("GETDATE()");
            builder.Property(u => u.Image).HasColumnType("varbinary(max)");
            builder.Property(u => u.Role).IsRequired();
            builder.Property(u => u.Status).IsRequired();
            builder.Property(u => u.language).IsRequired();
        }
    }
}
