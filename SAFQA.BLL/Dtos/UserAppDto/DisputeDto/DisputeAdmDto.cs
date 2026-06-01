using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.UserAppDto.DisputeDto
{
    public class DisputeAdmDto
    {
        public string? Description { get; set; }
        public string Reason { get; set; }
        public List<byte[]> Evidences { get; set; }
    }
}