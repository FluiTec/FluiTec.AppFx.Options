using System;
using Microsoft.Extensions.Configuration;

namespace FluiTec.AppFx.Options.Managers
{
    /// <summary>A reporting ConfigurationManager.</summary>
    /// <seealso cref="FluiTec.AppFx.Options.Managers.ConfigurationManager" />
    public class ReportingConfigurationManager : ConfigurationManager
    {
        #region Fields

        protected readonly Action<string> ReportAction;

        #endregion

        public string ConfigurationKeyReport { get; set; }

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="ReportingConfigurationManager"/> class.</summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="reportAction">The report action.</param>
        /// <exception cref="System.ArgumentNullException">reportAction</exception>
        public ReportingConfigurationManager(IConfigurationRoot configuration, Action<string> reportAction) : base(configuration)
        {
            ReportAction = reportAction ?? throw new ArgumentNullException(nameof(reportAction));

            // setup reports
            ConfigurationKeyReport = "[ConfigurationKey]: '{0}'";
        }

        #endregion

        #region Naming

        /// <summary>Gets the ConfigurationKey using the given type.</summary>
        /// <param name="type">The type to inspect.</param>
        /// <exception cref="System.ArgumentNullException">type</exception>
        /// <returns>The ConfigurationKey</returns>
        /// <remarks>
        /// Behavior:
        /// a) Will check internal cache if the the type was ever checked (and return accordingly)
        /// b) Will inspect the ConfigurationKeyAttribute (cache and return accordingly)
        /// c) Will use nameof(type) (cache and return accordingly)
        /// d) Will report the ConfigurationKey
        /// </remarks>
        public override string GetKeyByType(Type type)
        {
            var key = base.GetKeyByType(type);
            ReportAction(string.Format(ConfigurationKeyReport, key));
            return key;
        }

        #endregion
    }
}