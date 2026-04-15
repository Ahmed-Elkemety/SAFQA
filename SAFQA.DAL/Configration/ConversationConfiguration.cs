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
    public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
    {
        public void Configure(EntityTypeBuilder<Conversation> builder)
        {
            builder.ToTable("Conversations");

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Buyer)
                   .WithMany()
                   .HasForeignKey(x => x.BuyerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SellerUser)
                   .WithMany()
                   .HasForeignKey(x => x.SellerUserId)
                   .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(x => x.Dispute)
                   .WithOne()
                   .HasForeignKey<Conversation>(x => x.DisputeId)
                   .OnDelete(DeleteBehavior.Cascade);


            builder.HasMany(x => x.Messages)
                   .WithOne(x => x.Conversation)
                   .HasForeignKey(x => x.ConversationId)
                   .OnDelete(DeleteBehavior.Cascade);

  
            builder.Property(x => x.LastMessage)
                   .HasMaxLength(1000);

            builder.Property(x => x.Type)
                   .IsRequired();

            builder.Property(x => x.CreatedAt)
                   .IsRequired();

            builder.HasIndex(x => x.DisputeId)
                   .IsUnique();
        }
    }
}
