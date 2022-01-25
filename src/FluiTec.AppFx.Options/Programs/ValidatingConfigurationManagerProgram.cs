using FluiTec.AppFx.Options.Managers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FluiTec.AppFx.Options.Programs;

/// <summary>
/// A validating configuration manager program.
/// </summary>
public abstract class ValidatingConfigurationManagerProgram : ConfigurationManagerProgram
{
    /// <summary>
    /// Gets service provider.
    /// </summary>
    ///
    /// <returns>
    /// The service provider.
    /// </returns>
    protected override ServiceProvider GetServiceProvider()
    {
        var sp = base.GetServiceProvider();
        sp.UseSettingsValidator(Manager);
        return sp;
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
    protected override ValidatingConfigurationManager GetConfigurationManager(IConfigurationRoot configurationRoot)
    {
        return new ValidatingConfigurationManager(configurationRoot);
    }
}