using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EasySell.Startup))]
namespace EasySell
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
