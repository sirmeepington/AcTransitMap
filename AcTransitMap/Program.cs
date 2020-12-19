using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace AcTransitMap
{
    public class Program
    {
        public static void Main(string[] args)
        {

            LoggerConfiguration config = new LoggerConfiguration()
                .WriteTo.Console();
            string seqApiKey = Environment.GetEnvironmentVariable("SEQ_API_KEY");
            string seqUrl = Environment.GetEnvironmentVariable("SEQ_URL");
            if (!string.IsNullOrEmpty(seqApiKey) && !string.IsNullOrEmpty(seqUrl))
                config.WriteTo.Seq(seqUrl, apiKey: seqApiKey);

            Log.Logger = config
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft",Serilog.Events.LogEventLevel.Warning)
                .CreateLogger();
                
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddSerilog();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
