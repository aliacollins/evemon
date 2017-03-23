using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVEMon.Services.EVEAccountService.Entities
{
    [Serializable]
    internal class EVEAccountInfo
    {
        public string AccountName { get; set; }
        public string AuthorisationToken { get; set; }
        public string AuthenticationToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
