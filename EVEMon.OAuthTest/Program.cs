using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EVEMon.Entities.Events;
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

			Console.WriteLine("You should have been punched out!");

			// Subscribe to the SSOComplete event
			GlobalEvents.SSOComplete += SSOComplete;

			Console.ReadLine();
		}

		private static void SSOComplete(object sender, SSOCompleteEventArgs args)
		{
			Console.WriteLine("We have a response as follows:");
			Console.WriteLine($"Access token: {args.AccessToken}.");
			Console.WriteLine($"Auth token: {args.AuthorizationToken}.");
			Console.WriteLine($"Refresh token: {args.RefreshToken}.");
			Console.WriteLine($"Expires: {args.Expires}.");
		}
	}
}
