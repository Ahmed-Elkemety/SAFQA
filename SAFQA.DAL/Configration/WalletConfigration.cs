using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Models;

namespace SAFQA.DAL.Configration
{
    public class WalletConfigration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Wallet> builder)
        {
            builder
                .HasOne(a => a.User)
                .WithOne(e => e.wallet)
                .HasForeignKey<User>(a => a.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(a => a.Transactions)
                .WithOne(e => e.Wallet)
                .HasForeignKey(a => a.WalletId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Property(w => w.Balance).HasColumnType("decimal(18,2)").IsRequired().HasDefaultValue(0);
            builder.Property(w => w.FrozenBalance).HasColumnType("decimal(18,2)").IsRequired().HasDefaultValue(0);
            builder.Property(w => w.UpdatedAt).HasDefaultValueSql("GETDATE()").IsRequired();
        }
    }
}
