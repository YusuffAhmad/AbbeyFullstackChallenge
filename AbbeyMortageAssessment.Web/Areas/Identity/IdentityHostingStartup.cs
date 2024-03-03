using AbbeyMortageAssessment.Web.Areas.Identity;
using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(IdentityHostingStartup))]
namespace AbbeyMortageAssessment.Web.Areas.Identity
{

    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}