﻿using Microsoft.Extensions.Hosting;
using MIFCore;
using MIFCore.Http;

namespace MAD.DataWarehouse.Xero.XPM
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            IntegrationHost.CreateDefaultBuilder(args)
                .UseAspNetCore()
                .UseStartup<Startup>();
    }
}
