using Owin;

using EVEMon.Gateways.Extensions.Owin;

namespace EVEMon.OAuthTest
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseStaticContent();
            app.UseRedirectionEndPoints();
        }
    }
}