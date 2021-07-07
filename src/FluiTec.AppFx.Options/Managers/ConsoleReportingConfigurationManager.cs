using Microsoft.Extensions.Configuration;

namespace FluiTec.AppFx.Options.Managers
{
    /// <summary>A ConfigurationManager to reports to the console.</summary>
    /// <seealso cref="FluiTec.AppFx.Options.Managers.ReportingConfigurationManager" />
    public class ConsoleReportingConfigurationManager : ReportingConfigurationManager
    {
        /// <summary>Initializes a new instance of the <see cref="ConsoleReportingConfigurationManager" /> class.</summary>
        /// <param name="configuration">The configuration.</param>
        public ConsoleReportingConfigurationManager(IConfigurationRoot configuration) : base(configuration,
            System.Console.WriteLine)
        {
        }
    }
}