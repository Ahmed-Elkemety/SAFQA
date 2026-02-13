using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Configration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using SAFQA.DAL.Models;

   public class PendingUserRegistrationConfig : IEntityTypeConfiguration<PendingUserRegistration>
{
    public void Configure(EntityTypeBuilder<PendingUserRegistration> builder)
    {
        // Primary Key
        builder.HasKey(x => x.Id);

        // الخصائص المطلوبة
        builder.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.PasswordHash)
            .IsRequired();

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(x => x.Gender);

        builder.Property(x => x.BirthDate);

        builder.Property(x => x.CityId);

        builder.Property(x => x.OtpHash)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.OtpExpiration)
            .IsRequired();

        builder.Property(x => x.IsUsed)
            .IsRequired()
            .HasDefaultValue(false);

        // Index على الايميل لتسريع البحث
        builder.HasIndex(x => x.Email).IsUnique();
    }
}

}
