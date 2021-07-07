using System;
using System.IO;
using System.Linq;
using FluiTec.AppFx.Options.ConsoleSample.Configuration;
using FluiTec.AppFx.Options.Helpers;
using FluiTec.AppFx.Options.Managers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FluiTec.AppFx.Options.ConsoleSample
{
    /// <summary>   A program. </summary>
    internal class Program
    {
        private static readonly Random Random = new Random();

        /// <summary>   Main entry-point for this application. </summary>
        private static void Main()
        {
            var path = DirectoryHelper.GetApplicationRoot();
            System.Console.WriteLine($"BasePath: {path}");
            var config = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("appsettings.json", false, true).Build();

            var manager = new ConsoleReportingConfigurationManager(config);
            var services = new ServiceCollection();
            services.Configure<ApplicationSettings>(manager);
            manager.ConfigureValidator(new ApplicationSettingsValidator());

            var sp = services.BuildServiceProvider();
            sp.UseSettingsValidator(manager);
            sp.GetService<IOptionsMonitor<ApplicationSettings>>().OnChange(settings =>
                System.Console.WriteLine($"-> Monitor detected FileChange - new name: {settings.Name}"));

            System.Console.WriteLine("Type <Enter> to exit.");
            System.Console.WriteLine("Type a non empty string to update appsettings.json");
            while (System.Console.ReadLine() != string.Empty)
            {
                File.WriteAllText(Path.Combine(path, "appsettings.json"),
                    $"{{\"AppSettings\": {{\"Name\": \"{RandomString()}\"}}}}");

                var singleton = sp.GetService<IOptions<ApplicationSettings>>();
                System.Console.WriteLine($"Singleton using IOptions: {singleton.Value.Name}");
                var scoped = sp.CreateScope().ServiceProvider.GetService<IOptionsSnapshot<ApplicationSettings>>();
                System.Console.WriteLine($"Scoped using IOptionsSnapshot: {scoped.Value.Name}");
            }
        }

        /// <summary>   Random string. </summary>
        /// <param name="length">   (Optional) The length. </param>
        /// <returns>   A string. </returns>
        public static string RandomString(int length = 10)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}