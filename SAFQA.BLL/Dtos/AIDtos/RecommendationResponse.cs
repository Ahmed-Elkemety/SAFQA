using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFQA.BLL.Dtos.AIDtos
{
    public class RecommendationResponse
    {
        public string UserId { get; set; }
        public int Count { get; set; }
        public List<RecommendationDto> Recommendations { get; set; }
    }
}
