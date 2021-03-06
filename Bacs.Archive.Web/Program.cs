﻿using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Bacs.Archive.Web
{
    // ReSharper disable once UnusedMember.Global
    public class Program
    {
        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once UnusedParameter.Global
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }
    }
}
