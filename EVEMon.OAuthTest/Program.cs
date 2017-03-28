using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EVEMon.Gateways.EVEAuthGateway;
using EVEMon.Gateways.EVEAuthGateway.Specification;

namespace EVEMon.OAuthTest
{
	class Program
	{
		static void Main(string[] args)
		{
			IEVEAuthGateway AuthGateway = null;

			Console.WriteLine("The app is running...");

			Console.WriteLine("Calling the factory to produce an SSO Gateway:");

			try
			{
				// Running the tool should launch a browser (tab) and run straight to the OAuth page at CCP
				AuthGateway = EVEAuthFactory.GetEVEAuthGateway();
			}
			catch (Exception ex)
			{
				Console.WriteLine("There was a problem creating the Gateway...");
				Console.Write(ex.ToString());
			}

			Console.WriteLine("Calling the Authenticate method...");

			string ExecuteString = string.Empty;
			do
			{
				try
				{
					AuthGateway.AuthenticateWithSSO();
				}
				catch (Exception ex)
				{ }

				ExecuteString = Console.ReadLine();
			}
			while (ExecuteString == "Execute");
		}
	}
}
