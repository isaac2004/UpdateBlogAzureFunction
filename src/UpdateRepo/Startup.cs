using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UpdateRepo;

[assembly: WebJobsStartup(typeof(Startup))]
namespace UpdateRepo
{
    internal class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            ConfigureServices(builder.Services).BuildServiceProvider(true);
        }

        private IServiceCollection ConfigureServices(IServiceCollection services)
        {

            services.AddSingleton(typeof(ConfigWrapper));
            services.AddLogging(logging =>
            {
                logging.AddFilter(level => true);
            });

            return services;
        }
    }
}


