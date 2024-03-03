namespace AbbeyMortageAssessment.Web.Infrastructure
{
    using Data;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder MigrateDatabase(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            var db = scope.ServiceProvider.GetService<ApplicationDbContext>();

            db.Database.Migrate();

            return app;
        }
    }
}
