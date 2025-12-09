using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Models;

namespace SAFQA.DAL.Configration
{
    public class CountryConfigration : IEntityTypeConfiguration<Country>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Country> builder)
        {
            builder
                .HasMany(a => a.users)
                .WithOne(e => e.Country)
                .HasForeignKey(e => e.CountryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(c => c.Name).IsRequired().HasMaxLength(150);
        }
    }
}
