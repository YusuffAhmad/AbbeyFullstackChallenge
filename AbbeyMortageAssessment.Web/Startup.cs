namespace AbbeyMortageAssessment.Web
{
    using Infrastructure;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using AbbeyMortageAssessment.Data;
    using AbbeyMortageAssessment.Services.JSON;

    public class Startup
    {
        public Startup(IConfiguration configuration)
            => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddRazorPages();
            services.AddMvc();

            services
                .AddDbContext<ApplicationDbContext>(opt => opt
                    .UseSqlServer(Configuration.GetConnectionString("AbbeyMortageAssessmentDb")));

            services.AddIdentity();

            services.AddConventionalServices();
            // Add Generic service
            services.AddTransient(typeof(IJsonService<>), typeof(JsonService<>));

            // Cookies for Login
            services
                .ConfigureApplicationCookie(options => options
                    .LoginPath = "/Account/Login");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });

            app.MigrateDatabase();
        }
    }
}
