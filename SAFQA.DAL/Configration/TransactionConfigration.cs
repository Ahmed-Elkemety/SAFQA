using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Models;

namespace SAFQA.DAL.Configration
{
    public class TransactionConfigration : IEntityTypeConfiguration<Transactions>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Transactions> builder)
        {

            builder
                .HasOne(a => a.Wallet)
                .WithMany(e => e.Transactions)
                .HasForeignKey(a => a.WalletId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Property(t => t.ReferenceId).HasMaxLength(100);
            builder.Property(t => t.Amount).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(t => t.BalanceBefore).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(t => t.BalanceAfter).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(t => t.Description).HasMaxLength(500);
            builder.Property(t => t.CreatedAt).HasDefaultValueSql("GETDATE()");
            builder.HasIndex(t => t.Type);
            builder.HasIndex(t => t.Status);
            builder.HasIndex(t => t.WalletId);
            builder.HasIndex(t => t.CreatedAt);
        }
    }
}
