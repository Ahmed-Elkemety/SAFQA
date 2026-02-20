using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Models;

namespace SAFQA.DAL.Configration
{
    public class SavedCardConfiguration : IEntityTypeConfiguration<SavedCard>
    {
        public void Configure(EntityTypeBuilder<SavedCard> builder)
        {
            builder.ToTable("SavedCards");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Last4Digits)
                   .IsRequired()
                   .HasMaxLength(4);

            builder.Property(c => c.CardBrand)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(c => c.PaymentToken)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(c => c.ExpiryMonth)
                   .IsRequired();

            builder.Property(c => c.ExpiryYear)
                   .IsRequired();

            builder.Property(c => c.IsDefault)
                   .HasDefaultValue(false);

            builder.Property(c => c.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            // العلاقة مع Wallet
            builder.HasOne(c => c.Wallet)
                   .WithMany(w => w.SavedCards)
                   .HasForeignKey(c => c.WalletId)
                   .OnDelete(DeleteBehavior.Cascade);

            // منع تكرار نفس الكارت لنفس الـ Wallet
            builder.HasIndex(c => new { c.WalletId, c.PaymentToken })
                   .IsUnique();
        }
    }
}
