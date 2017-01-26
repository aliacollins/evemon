using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVEMon.Gateways.EVEAuthGateway.Specification
{
	public interface IEVEAuthGateway
	{
		string AuthenticateWithSSO();
	}
}
