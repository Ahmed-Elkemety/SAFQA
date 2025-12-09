using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Models;

namespace SAFQA.DAL.Configration
{
    public class CityConfigration : IEntityTypeConfiguration<City>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<City> builder)
        {
            builder
                .HasOne(a => a.Country)
                .WithMany(e => e.Cities)
                .HasForeignKey(e => e.CountryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(a => a.users)
                .WithOne(e => e.City)
                .HasForeignKey(a => a.CityId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(c => c.Name).IsRequired().HasMaxLength(150);
        }
    }
}
