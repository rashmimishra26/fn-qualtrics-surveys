using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using qualtrics_surveys.Implementation;
using qualtrics_surveys.Interfaces;
using qualtrics_surveys.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using static System.Console;

[assembly: FunctionsStartup(typeof(qualtrics_surveys.RegisterServices))]
namespace qualtrics_surveys
{
    internal class RegisterServices: FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {

            IConfiguration configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName)
                            .AddJsonFile("azuresettings.json", optional: false, reloadOnChange: true)
                            .AddJsonFile("local.settings.json", optional: true)
                            .Build();

            builder.Services.AddOptions();
            builder.Services.Configure<AzureSettings>(configuration);
            builder.Services.AddTransient<IConnect2Azure, Connect2Azure>(obj => new Connect2Azure(obj.GetService<IOptions<AzureSettings>>()));
            builder.Services.AddTransient<IAuth, Auth>(svc => new Auth(svc.GetService<IConnect2Azure>(), svc.GetService<IOptions<AzureSettings>>()));
            builder.Services.AddTransient<IUtil, Util>(svc => new Util(svc.GetService<IConnect2Azure>(), svc.GetService<IOptions<AzureSettings>>(), svc.GetService<IAuth>()));
            //builder.Services.AddScoped(hc => new HttpClient { BaseAddress = new Uri("https://jsonplaceholder.typicode.com/") });

        }
    }
}
