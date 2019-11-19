using Amazon.Runtime;
using Amazon.SQS;
using AwsSqs.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;

namespace AwsSqs.Api
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
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Dotnet + AWS SQS - API",
                    Version = "v1",
                    Description = "Com essa API iremos publicar mensagens na fila do SQS",
                    Contact = new OpenApiContact
                    {
                        Name = "Levy Furst Neto",
                        Url = new Uri("https://medium.com/@levyfurst")
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            var awsOptions = Configuration.GetAWSOptions();
            awsOptions.Credentials = new BasicAWSCredentials(Configuration["AWS:Access_Key"],
                Configuration["AWS:Secret_Access_Key"]);

            services.AddSingleton(Configuration);

            services.AddSingleton<ISqsService>(provider =>
                new SqsService(awsOptions.CreateServiceClient<IAmazonSQS>()));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dotnet + AWS SQS - API - V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
