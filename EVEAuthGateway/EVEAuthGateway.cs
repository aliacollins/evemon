using System;
using System.Diagnostics;

using Flurl;
using Microsoft.Owin.Hosting;

using EVEMon.Gateways.Extensions.Owin;
using EVEMon.Gateways.Properties;
using EVEMon.Gateways.Specification;

namespace EVEMon.Gateways
{
	public class EVEAuthGateway : IEVEAuthGateway
	{
		private IDisposable WebServer;

		/// <summary>
		/// A constructor which initialises the WebApp required for the authentication process
		/// </summary>
		public EVEAuthGateway()
		{
			Settings AppSettings = Settings.Default;
			string BaseURL = AppSettings.InternalServerBaseURL;
			bool StartSuccess = StartWebApp(BaseURL);

			if (!StartSuccess)
				throw new NotImplementedException(); // TODO - Ashilta - Need to do some error things in here

			try
			{ Process.Start(BaseURL.AppendPathSegment("Startup")); }
			catch (Exception ex)
			{ } // TODO - Ashilta - Need to do some error things in here!
		}

		private IDisposable OwinServer;

		/// <summary>
		/// Perform any cleanup actions with the API we're using, dispose of resources and tidy up cleanly. 
		/// </summary>
		public void Dispose()
		{
			OwinServer.Dispose();
			// TODO - Ashilta - Need to progmmatically unreserve the port that was used for the startup process so that we know it's free for A) other applications or B) next time
		}

		/// <summary>
		/// Starts a locally run WebApp, required to receive the responses of calls to any EVE API or SSO process
		/// </summary
		private bool StartWebApp(string baseURL)
		{
			try
			{
				OwinServer = WebApp.Start<OwinStartupConfig>(baseURL);
			}
			catch(Exception ex)
			{
				// TODO - Ashilta - Call the error logging stuff that we normally use. I need to look into this.
				// TODO - Ashilta - Catch the specific exception that's thrown when trying to start using a port that's already been reserved. It can retry.
				return false;
			}

			return true;
		}
	}
}