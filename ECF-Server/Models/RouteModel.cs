using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECF_Server.Models
{
    public class RouteModel
    {
        public AddressList addressList { get; set; }
        public long[,] timeWindows { get; set; }
    }
}
