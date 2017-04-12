using System;

using EVEMon.Gateways.Specification;

namespace EVEMon.Gateways.Factories
{
	/// <summary>
	/// Based on the information required, determines which API to use and returns a class to interact with it.	
	/// </summary>
    public static class EVEAuthFactory
    {
		public static IEVEAuthGateway GetEVEAuthGateway()
		{
			return new EVEAuthGateway();
		}
    }
}
