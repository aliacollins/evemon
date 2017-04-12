using System;
using System.Collections.Generic;
using System.Web.Http;

using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Microsoft.Owin.StaticFiles.ContentTypes;
using Owin;

namespace EVEMon.Gateways.Extensions.Owin
{
	public static class OwinExtensions
	{
		public static IAppBuilder UseRedirectionEndPoints(this IAppBuilder app)
		{
			// Configure Web API for self-host. 
			var config = new HttpConfiguration();
			config.Routes.MapHttpRoute(
				"Authentication",
				"{controller}"
			);

			return app.UseWebApi(config);
		}

		public static IAppBuilder UseStaticContent(this IAppBuilder app)
		{
			var contentTypes = new Dictionary<string, string> {
				{".html", "text/html"},
				{".jpeg", "image/jpeg"},
				{".png", "image/png"},
				{".css", "text/css"}
			};

			// Configure static file hosting
			var options = new FileServerOptions
			{
				EnableDefaultFiles = false,
				EnableDirectoryBrowsing = false,
				FileSystem =
					new EmbeddedResourceFileSystem(typeof(OwinStartupConfig).Assembly, "EVEOnlineSingleSignOnWinForms.Content"),
				StaticFileOptions = {
					ContentTypeProvider = new FileExtensionContentTypeProvider(contentTypes)
				}
			};

			return app.UseFileServer(options);
		}
	}
}