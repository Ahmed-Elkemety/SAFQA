using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SAFQA.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.DAL.Configration
{
    public class MessageAttachmentConfiguration : IEntityTypeConfiguration<MessageAttachment>
    {
        public void Configure(EntityTypeBuilder<MessageAttachment> builder)
        {
            builder.ToTable("MessageAttachments");

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Message)
                   .WithMany(x => x.Attachments)
                   .HasForeignKey(x => x.MessageId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.FileUrl)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(x => x.FileType)
                   .IsRequired()
                   .HasMaxLength(50);

          
            builder.Property(x => x.Size)
                   .IsRequired();

            builder.Property(x => x.CreatedAt)
                   .IsRequired();
        }
    }
}
