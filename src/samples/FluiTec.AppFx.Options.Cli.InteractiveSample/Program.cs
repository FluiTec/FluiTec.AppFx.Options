using FluiTec.AppFx.Console;
using FluiTec.AppFx.Console.Configuration;
using FluiTec.AppFx.Options.Cli.InteractiveSample.Configuration;
using FluiTec.AppFx.Options.Console;
using FluiTec.AppFx.Options.Programs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FluiTec.AppFx.Options.Cli.InteractiveSample;

/// <summary>
///     A program.
/// </summary>
internal class Program : ValidatingConfigurationManagerProgram
{
    /// <summary>
    ///     Main entry-point for this application.
    /// </summary>
    /// <param name="args"> An array of command-line argument strings. </param>
    private static void Main(string[] args)
    {
        var serviceProvider = new Program().GetServiceProvider();
        new ConsoleHost(serviceProvider).RunInteractive("Test", args);
    }

    /// <summary>
    /// Configures the given configuration builder.
    /// </summary>
    ///
    /// <param name="configurationBuilder"> The configuration builder. </param>
    ///
    /// <returns>
    /// An IConfigurationBuilder.
    /// </returns>
    protected override IConfigurationBuilder Configure(IConfigurationBuilder configurationBuilder)
    {
        return configurationBuilder
            .AddJsonFile("appsettings.json", false, true)
            .AddSaveableJsonFile("appsettings.conf.json", false, true);
    }

    /// <summary>
    /// Configure services.
    /// </summary>
    ///
    /// <param name="services"> The services. </param>
    ///
    /// <returns>
    /// A ServiceCollection.
    /// </returns>
    protected override ServiceCollection ConfigureServices(ServiceCollection services)
    {
        base.ConfigureServices(services);

        services.Configure<ApplicationSettings>(Manager);
        Manager.ConfigureValidator(new ApplicationSettingsValidator());

        services.ConfigureOptionsConsoleModule();

        return services;
    }
}