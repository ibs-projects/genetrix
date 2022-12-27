using Microsoft.Owin;
using Owin;
using System.Globalization;
using System.Threading;

namespace genetrix
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
