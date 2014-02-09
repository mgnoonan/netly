using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(netly.Startup))]
namespace netly
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
