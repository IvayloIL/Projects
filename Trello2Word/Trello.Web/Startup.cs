using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Trello.Web.Startup))]
namespace Trello.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
