using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;
using Byndyusoft.Data.Relational.QueryBuilder.Sample.Extensions;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Byndyusoft.Data.Relational.QueryBuilder.Sample
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen();

            services.AddSampleApplication(Configuration);
            services.AddRelationalDb(NpgsqlFactory.Instance, Configuration.GetConnectionString("postgres"));

            services.AddControllers()
                .AddJsonOptions(json =>
                {
                    json.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger()
                .UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Sample API");
                    options.DisplayRequestDuration();
                    options.DefaultModelRendering(ModelRendering.Model);
                    options.DefaultModelExpandDepth(3);
                });

            app
                .UseRouting()
                .UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
