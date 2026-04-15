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
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Messages");

            builder.HasKey(x => x.Id);

        
            builder.HasOne(x => x.Conversation)
                   .WithMany(x => x.Messages)
                   .HasForeignKey(x => x.ConversationId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Sender)
                   .WithMany()
                   .HasForeignKey(x => x.SenderId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Content)
                   .IsRequired()
                   .HasMaxLength(2000);

            builder.Property(x => x.IsSeen)
                   .HasDefaultValue(false);

       
            builder.Property(x => x.CreatedAt)
                   .IsRequired();


            builder.HasIndex(x => x.ConversationId);
        }
    }
}
