using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBM.Common.Constants
{
    public static class ApplicationConstants
    {
        public static readonly List<string> AllowedDomainList;
        static ApplicationConstants()
        {
            AllowedDomainList = new List<string>() { "localhost", "mybestman" };
        }
    }
}
