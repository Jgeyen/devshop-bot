using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium.Chrome;

namespace devshop_server {


    public class Program {
        public static Driver Driver;
        public static void Main(string[] args) {
            Driver = GetStarted();
            
            CreateHostBuilder(args).Build().Run();
        }
//Help
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                });

        private static Driver GetStarted() {
            var rootDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).Substring(6);
            var options = new ChromeOptions();
            options.AddArgument("start-maximized");
            options.AddArgument("no-sandbox");

            var driver = new ChromeDriver(rootDir, options, TimeSpan.FromMinutes(3));

            driver.Navigate().GoToUrl("https://secretgeek.github.io/devShop/");
            driver.FindElementById("start").Click();
            return new Driver(driver);
        }
    }
}
