using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Owin.Hosting;

using EVEMon.Gateways.EVEAuthGateway.Specification;

namespace EVEMon.Gateways.EVEAuthGateway
{
	public class EVEAuthGateway : IEVEAuthGateway, IDisposable
	{
		/// <summary>
		/// A constructor which initialises the WebApp required for the authentication process
		/// </summary>
		public EVEAuthGateway()
		{
			StartWebApp();
		}

		private IDisposable OwinServer;

		public string AuthenticateWithSSO()
		{ return string.Empty; }

		/// <summary>
		/// Perform any cleanup actions with the API we're using, dispose of resources and tidy up cleanly. 
		/// </summary>
		public void Dispose()
		{
			OwinServer.Dispose();
		}

		/// <summary>
		/// Starts a locally run WebApp, required to receive the responses of calls to any EVE API
		/// </summary
		private bool StartWebApp()
		{
			try
			{
				string BaseURL = string.Empty;
				OwinServer = WebApp.Start<OwinStartupConfig>(BaseURL);
			}
			catch (Exception ex)
			{
				// AJA - Call the error logging stuff that we normally use. I need to look into this.
				return false;
			}

			return true;
		}
	}
}
