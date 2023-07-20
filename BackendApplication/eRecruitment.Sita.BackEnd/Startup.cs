using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(eRecruitment.Sita.BackEnd.Startup))]
namespace eRecruitment.Sita.BackEnd
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
