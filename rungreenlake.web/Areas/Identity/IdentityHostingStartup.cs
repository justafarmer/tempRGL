using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using rungreenlake.web.Areas.Identity.Data;

[assembly: HostingStartup(typeof(rungreenlake.web.Areas.Identity.IdentityHostingStartup))]
namespace rungreenlake.web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<Context>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("DefaultConnection"),
                        x => x.MigrationsAssembly("rungreenlake.web")));

                //services.AddDefaultIdentity<RunGreenLakeUser>(options => options.SignIn.RequireConfirmedAccount = true)
                //    .AddEntityFrameworkStores<Context>();
            });
        }
    }
}