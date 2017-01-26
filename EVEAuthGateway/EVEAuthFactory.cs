using System;

using EVEMon.Gateways.EVEAuthGateway.Specification;

namespace EVEMon.Gateways.EVEAuthGateway
{
	/// <summary>
	/// Based on the information required, determines which API to use and returns a class to interact with it.	
	/// </summary>
    public class EVEAuthFactory
    {
		public IEVEAuthGateway GetEVEAuthGateway()
		{
			return new EVEAuthGateway();
		}
    }
}
