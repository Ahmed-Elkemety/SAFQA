using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace SAFQA.BLL.Managers.AccountManager
{

    public class FacebookDebugResponse
    {
        [JsonPropertyName("data")]
        public FacebookDebugData Data { get; set; }
    }

    public class FacebookDebugData
    {
        [JsonPropertyName("is_valid")]
        public bool IsValid { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }
    }
}
