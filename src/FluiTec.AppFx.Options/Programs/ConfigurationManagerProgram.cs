using FluiTec.AppFx.Console.Programs;
using FluiTec.AppFx.Options.Managers;
using Microsoft.Extensions.Configuration;

namespace FluiTec.AppFx.Options.Programs;

/// <summary>
/// A configuration manager program.
/// </summary>
public abstract class ConfigurationManagerProgram : ConfigurableDependencyInjectionProgram
{
    /// <summary>
    /// Gets or sets the manager.
    /// </summary>
    ///
    /// <value>
    /// The manager.
    /// </value>
    protected ValidatingConfigurationManager Manager { get; set; }

    protected override IConfigurationRoot GetConfiguration()
    {
        var conf = base.GetConfiguration();
        Manager = GetConfigurationManager(conf);
        return conf;
    }

    /// <summary>
    /// Gets configuration manager.
    /// </summary>
    ///
    /// <param name="configurationRoot">    The arguments configuration root. </param>
    ///
    /// <returns>
    /// The configuration manager.
    /// </returns>
    protected abstract ValidatingConfigurationManager GetConfigurationManager(IConfigurationRoot configurationRoot);
}