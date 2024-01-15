using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace MBM.Common.Models.Api.Response
{
    public class RegisterResponse
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
