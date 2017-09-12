using System;

using IO.Swagger.Client;
using IO.Swagger.Model;
using IO.Swagger.Api;

namespace EVEMon.Entities.ServerStatus
{
	public static class ServerStatus
	{

		public static int PilotsOnline { get; set; }
		public static ServerMode CurrentServerMode { get; set; }
		public static string ServerVersion { get; set; }
		private static DateTime CacheExpires { get; set; }

		static ServerStatus()
		{
			CacheExpires = DateTime.UtcNow.AddSeconds(-1);
			GetServerStatus();
		}

		// invoked by events, updates it's internal model if appropriate, and then pushes to needed places
		public static void onEvent()
		{
			GetServerStatus();
			PushStatusViewToMainWindowFooter();
		}

		// This shouldn't be here, it's the "viewmodel" from MVVM pattern, and should definitely be refactored elsewhere
		private static void PushStatusViewToMainWindowFooter()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Using the Gateway, get and update the server status variables
		/// </summary>
		/// <returns></returns>
		private static void GetServerStatus()
		{
			if (!(DateTime.Now > CacheExpires))
				return;

			StatusApi API = new StatusApi();
			if (API == null)
			{
				// TODO - Decide on an error handling strategy; do we warn the user or just log it?
				SetDefaultValues();
				return;
			}

			ApiResponse<GetStatusOk> Status = null;
			try
			{
				Status = API.GetStatusWithHttpInfo();
			}
			catch (ApiException)
			{
				// TODO - Perform relevant logging operations
				SetDefaultValues();
				return;
			}

			if (Status.Data == null)
			{
				// TODO - Perform relevant logging operations
				SetDefaultValues();
				return;
			}

			string rawExpires = string.Empty;
			if (Status.Headers.TryGetValue("expires", out rawExpires))
			{
				DateTime Expires;
				bool ParseSuccess = DateTime.TryParse(rawExpires, out Expires);

				CacheExpires = ParseSuccess ? Expires : DateTime.Now.AddSeconds(30);
			}

			PilotsOnline = Status.Data.Players.Value;

			if (Status.Data.Vip.HasValue && Status.Data.Vip.Value == true)
				CurrentServerMode = ServerMode.VIP;

			ServerVersion = Status.Data.ServerVersion;
			// Triger some sort of event, to let whoever's interested know that the status has been updated.
		}

		/// <summary>
		/// Sets default, safe values for the ServerSTatus object reflecting an unknown server status
		/// </summary>
		private static void SetDefaultValues()
		{
			PilotsOnline = 0;
			CurrentServerMode = ServerMode.Unknown;
			ServerVersion = "Unknown";
			CacheExpires = DateTime.Now.AddSeconds(30);
		}

		/// <summary>
		/// Possible server modes.
		/// </summary>
		public enum ServerMode
		{
			// TODO - (How) can we detect Starting Up?
			Unknown,
			Offline,
			Online,
			VIP
		}
	}
}