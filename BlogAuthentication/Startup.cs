using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BlogAuthentication.Startup))]
namespace BlogAuthentication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
