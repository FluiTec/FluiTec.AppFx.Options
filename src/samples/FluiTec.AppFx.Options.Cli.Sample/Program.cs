using FluiTec.AppFx.Console;
using FluiTec.AppFx.Console.Configuration;
using FluiTec.AppFx.Options.Cli.Sample.Configuration;
using FluiTec.AppFx.Options.Console;
using FluiTec.AppFx.Options.Helpers;
using FluiTec.AppFx.Options.Managers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FluiTec.AppFx.Options.Cli.Sample
{
    /// <summary>
    /// A program.
    /// </summary>
    internal class Program
    {
        /// <summary>
        ///     Main entry-point for this application.
        /// </summary>
        /// <param name="args"> An array of command-line argument strings. </param>
        private static void Main(string[] args)
        {
            var serviceProvider = GetServicePovider();
            new ConsoleHost(serviceProvider).Run("Test", args);
        }

        /// <summary>
        ///     Gets the configuration.
        /// </summary>
        /// <returns>
        ///     The configuration.
        /// </returns>
        private static IConfigurationRoot GetConfiguration()
        {
            var path = DirectoryHelper.GetApplicationRoot();
            var config = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("appsettings.json", false, true)
                .AddSaveableJsonFile("appsettings.conf.json", false, true)
                .Build();
            return config;
        }

        /// <summary>
        ///     Gets service povider.
        /// </summary>
        /// <returns>
        ///     The service povider.
        /// </returns>
        private static ServiceProvider GetServicePovider()
        {
            var conf = GetConfiguration();
            var manager = new ValidatingConfigurationManager(conf);
            var services = new ServiceCollection();

            services.Configure<ApplicationSettings>(manager);
            manager.ConfigureValidator(new ApplicationSettingsValidator());

            services.AddSingleton(conf);
            services.ConfigureOptionsConsoleModule();

            var sp = services.BuildServiceProvider();
            sp.UseSettingsValidator(manager);

            return sp;
        }
    }
}
