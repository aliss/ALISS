using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ALISS.Admin.Web.Startup))]
namespace ALISS.Admin.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
