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
                .WithOne(e => e.Wallet)
                .HasForeignKey<Wallet>(a => a.UserId);


            builder.Property(w => w.Balance).IsRequired().HasDefaultValue(0);
            builder.Property(w => w.FrozenBalance).IsRequired().HasDefaultValue(0);
            builder.Property(w => w.UpdatedAt).HasDefaultValueSql("GETDATE()").IsRequired();
        }
    }
}
