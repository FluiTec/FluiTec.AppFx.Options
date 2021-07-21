using System.Linq;
using FluiTec.AppFx.Console;
using FluiTec.AppFx.Console.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FluiTec.AppFx.Options.Console
{
    /// <summary>   The options console extensions. </summary>
    public static class OptionsConsoleExtensions
    {
        /// <summary>   Configure options console module. </summary>
        /// <param name="services"> The services. </param>
        public static void ConfigureOptionsConsoleModule(this IServiceCollection services)
        {
            ConsoleHost.ConfigureModule(services, provider =>
            {
                var conf = provider.GetRequiredService<IConfigurationRoot>();
                var cp = conf.Providers.Single(p => p is SaveableJsonConfigurationProvider);
                return new OptionsConsoleModule(cp);
            });
        }
    }
}