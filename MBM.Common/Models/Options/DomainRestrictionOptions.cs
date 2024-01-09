using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBM.Common.Models.Options
{
    public class DomainRestrictionOptions
    {
        public required string[] AllowedDomains { get; set; }
    }
}
