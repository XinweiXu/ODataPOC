using System.Linq;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OData.Edm;
using WeatherAPI.Models.Db;
using WeatherAPI.Models.Weather;
using Microsoft.EntityFrameworkCore;
using WeatherAPI.Repository;

namespace WeatherAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureChangeCommunicationDbContextAndStores(services);

            services.AddScoped<IMessagesRepository, MessagesRepository>();

            services.AddControllers(options => options.EnableEndpointRouting = false)
                .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );

            services.AddOData();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseMvc(routeBuilder =>
            {
                routeBuilder.EnableDependencyInjection();
                routeBuilder.Select().Expand().Filter().OrderBy().Count().MaxTop(10);
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.Select().Expand().Filter().OrderBy().Count().MaxTop(10);
                endpoints.MapODataRoute("odata", "odata", GetCCEdmModel());
            });

        }

        private IEdmModel GetCCEdmModel()
        {
            var odataBuilder = new ODataConventionModelBuilder();

            odataBuilder.EntitySet<Message>("Messages");
            odataBuilder.EntitySet<WeatherForecast>("WeatherForecast");

            var function = odataBuilder.Function("GetMessages");
            function.Parameter<int>("statusId");
            function.Parameter<string>("languageCode");
            function.ReturnsCollectionFromEntitySet<Message>("Messages");

            odataBuilder.EnableLowerCamelCase();
            var edmModel = odataBuilder.GetEdmModel();
            return edmModel;
        }

        private void ConfigureChangeCommunicationDbContextAndStores(IServiceCollection services)
        {
            if (!string.IsNullOrEmpty(Configuration.GetValue<string>("ChangeCommunication:ConnectionString")))
            {
                services.AddDbContext<ChangeCommunicationDbContext>(options =>
                {
                    options.UseNpgsql(Configuration.GetValue<string>("ChangeCommunication:ConnectionString"));
                    options.EnableSensitiveDataLogging();
                });
            }
        }
    }
}
