using Amazon.Runtime;
using Amazon.SQS;
using AwsSqs.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace AwsSqs.Worker
{
    public class Program
    {
        static IConfigurationRoot Configuration { get; set; }

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, configApp) => { BuildJsonConfig(configApp); })
                .ConfigureServices((hostContext, services) =>
                {
                    var awsOptions = hostContext.Configuration.GetAWSOptions();
                    awsOptions.Credentials = new BasicAWSCredentials(Configuration["AWS:Access_Key"],
                        Configuration["AWS:Secret_Access_Key"]);

                    services.AddSingleton(Configuration);
                    services.AddDefaultAWSOptions(awsOptions);
                    services.AddAWSService<IAmazonSQS>();
                    services.AddSingleton<ISqsService, SqsService>();
                    services.AddHostedService<Worker>();
                });

        private static void BuildJsonConfig(IConfigurationBuilder configApp)
        {
            configApp.AddJsonFile("appsettings.json", false, true);
            configApp.AddJsonFile(
                $"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", true);
            configApp.AddEnvironmentVariables();

            Configuration = configApp.Build();
        }
    }
}
