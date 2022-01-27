using System.IO;
using FluiTec.AppFx.Console.Helpers;
using FluiTec.AppFx.Options.ConsoleSample.Configuration;
using FluiTec.AppFx.Options.Programs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FluiTec.AppFx.Options.ConsoleSample;

/// <summary>   A program. </summary>
internal class Program : ValidatingConfigurationManagerProgram
{
    private static void Main()
    {
        var sp = new Program().GetServiceProvider();
        var path = DirectoryHelper.GetApplicationRoot();

        System.Console.WriteLine("Type <Enter> to exit.");
        System.Console.WriteLine("Type a non empty string to update appsettings.json");

        while (System.Console.ReadLine() != string.Empty)
        {
            WriteConfigFile(Path.Combine(path, "appsettings.json"), "MyApplication");

            var singleton = sp.GetService<IOptions<ApplicationSettings>>();
            if (singleton != null)
                System.Console.WriteLine($"Singleton using IOptions: {singleton.Value.Name}");

            var scoped = sp.CreateScope().ServiceProvider.GetService<IOptionsSnapshot<ApplicationSettings>>();
            if (scoped != null)
                System.Console.WriteLine($"Scoped using IOptionsSnapshot: {scoped.Value.Name}");
        }
    }

    /// <summary>
    ///     Writes a configuration file.
    /// </summary>
    /// <param name="path"> Full pathname of the file. </param>
    /// <param name="name"> The name. </param>
    private static void WriteConfigFile(string path, string name)
    {
        var content = $"{{\"AppSettings\": {{\"Name\": \"{name}\"}}}}";
        File.WriteAllText(path, content);
    }

    /// <summary>
    ///     Configures the given configuration builder.
    /// </summary>
    /// <param name="configurationBuilder"> The configuration builder. </param>
    /// <returns>
    ///     An IConfigurationBuilder.
    /// </returns>
    protected override IConfigurationBuilder Configure(IConfigurationBuilder configurationBuilder)
    {
        return configurationBuilder
            .AddJsonFile("appsettings.json", false, true);
    }

    /// <summary>
    ///     Configure services.
    /// </summary>
    /// <param name="services"> The services. </param>
    /// <returns>
    ///     A ServiceCollection.
    /// </returns>
    protected override ServiceCollection ConfigureServices(ServiceCollection services)
    {
        base.ConfigureServices(services);

        services.Configure<ApplicationSettings>(Manager);
        Manager.ConfigureValidator(new ApplicationSettingsValidator());

        return services;
    }
}