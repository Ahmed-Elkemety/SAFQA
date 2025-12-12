using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Models;

namespace SAFQA.DAL.Configration
{
    public class NotificationConfigration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Notification> builder)
        {
            builder
                .HasOne(a => a.User)
                .WithMany(e => e.Notifications)
                .HasForeignKey(a => a.UserId);


            builder.Property(n => n.Title).IsRequired().HasMaxLength(200);
            builder.Property(n => n.notificationType).IsRequired();
            builder.Property(n => n.ReferenceId).IsRequired();
            builder.Property(n => n.Message).HasMaxLength(2000);
            builder.Property(n => n.IsRead).HasDefaultValue(false);
            builder.Property(n => n.UserId).IsRequired();
        }
    }
}
