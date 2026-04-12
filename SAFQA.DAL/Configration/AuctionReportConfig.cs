using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SAFQA.DAL.Models;
using System.Reflection.Emit;

namespace SAFQA.DAL.Configration
{
    public class AuctionReportConfig : IEntityTypeConfiguration<AuctionReport>
    {
        public void Configure(EntityTypeBuilder<AuctionReport> builder)
        {
            builder.HasKey(x => new { x.AuctionId, x.UserId });
        }
    }
}
