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
            builder.ToTable("AspNetUsers");

            builder
                .HasOne(c => c.City)
                .WithMany(u => u.users)
                .HasForeignKey(c => c.CityId)
                .IsRequired(false);



            builder.Property(u => u.FullName).IsRequired().HasMaxLength(150);

            builder.Property(u => u.Gender).HasMaxLength(10);

            builder.Property(u => u.Image).HasColumnType("varbinary(max)").IsRequired(false);

            builder.Property(u => u.BirthDate).HasDefaultValueSql("GETDATE()");

            builder.Property(u => u.Role).IsRequired();

            builder.Property(u => u.Status).IsRequired();

            builder.Property(u => u.Language).IsRequired();

            builder.Property(u => u.CreatedAt).HasDefaultValueSql("GETDATE()");

            builder.Property(u => u.UpdatedAt).HasDefaultValueSql("GETDATE()");

            builder.Property(u => u.LastLogin).HasDefaultValueSql("GETDATE()");

            builder.Property(u => u.IsDeleted).HasDefaultValue(false);

            builder.Property(u => u.DeletedAt).HasMaxLength(50).IsRequired(false);
        }
    }
}
