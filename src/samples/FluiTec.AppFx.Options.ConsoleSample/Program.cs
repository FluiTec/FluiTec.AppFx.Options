using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using FluiTec.AppFx.Options.ConsoleSample.Configuration;
using FluiTec.AppFx.Options.Managers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FluiTec.AppFx.Options.ConsoleSample
{
    internal class Program
    {
        private static void Main()
        {
            var path = GetApplicationRoot();
            Console.WriteLine($"BasePath: {path}");
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
                Console.WriteLine($"-> Monitor detected FileChange - new name: {settings.Name}"));

            Console.WriteLine("Type <Enter> to exit.");
            Console.WriteLine("Type a non empty string to update appsettings.json");
            while (Console.ReadLine() != string.Empty)
            {
                File.WriteAllText(Path.Combine(path, "appsettings.json"),
                    $"{{\"AppSettings\": {{\"Name\": \"{RandomString()}\"}}}}");

                var singleton = sp.GetService<IOptions<ApplicationSettings>>();
                Console.WriteLine($"Singleton using IOptions: {singleton.Value.Name}");
                var scoped = sp.CreateScope().ServiceProvider.GetService<IOptionsSnapshot<ApplicationSettings>>();
                Console.WriteLine($"Scoped using IOptionsSnapshot: {scoped.Value.Name}");
            }
        }

        private static string GetApplicationRoot()
        {
            var exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            var appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            var appRoot = appPathMatcher.Match(exePath).Value;
            return appRoot;
        }

        private static readonly Random Random = new Random();

        public static string RandomString(int length = 10)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}
