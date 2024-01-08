using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBM.Common.Models.Api.Request
{
    public class TokenRequest
    {
        public required string UserId { get; set; }
        public required string Username { get; set; }
        public required string TokenSecretKey { get; set; }
    }
}
