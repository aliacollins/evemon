using Owin;

using EVEMon.Gateways.EVEAuthGateway;
using EVEMon.Utilities.Extensions.Owin;

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