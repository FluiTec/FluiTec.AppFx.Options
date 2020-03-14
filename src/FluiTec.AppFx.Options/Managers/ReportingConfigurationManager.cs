﻿using System;
using System.Linq;
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

        /// <summary>Gets or sets the configuration key report.</summary>
        /// <value>The configuration key report.</value>
        public string ConfigurationKeyReport { get; set; }

        /// <summary>Gets or sets the extract settings report.</summary>
        /// <value>The extract settings report.</value>
        public string ExtractSettingsReport { get; set; }

        /// <summary>Gets or sets the null settings report.</summary>
        /// <value>The null settings report.</value>
        public string NullSettingsReport { get; set; }

        /// <summary>Gets or sets the property report.</summary>
        /// <value>The property report.</value>
        public string PropertyReport { get; set; }

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
            ExtractSettingsReport =  "[ConfigurationSettings of '{0}':]";
            NullSettingsReport =     "-> No settings provided.";
            PropertyReport =         "-> '{0}' = '{1}'";
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

        /// <summary>Extracts the settings.</summary>
        /// <typeparam name="TSettings">The type of the settings.</typeparam>
        /// <returns>The settings.</returns>
        /// <remarks>
        /// Will get the required section as indicated by <see cref="GetKeyByType"/>
        /// and bind a new instance of <see cref="TSettings"/> to the section
        /// returning that instance. (no cache involved)
        /// This method should only be used for direct inspection of certain
        /// options, since it won't register any settings to any kind of
        /// ServiceCollection. Will also report extracted settings.
        /// </remarks>
        public override TSettings ExtractSettings<TSettings>()
        {
            var settings =  base.ExtractSettings<TSettings>();
            ReportAction(string.Format(ExtractSettingsReport, typeof(TSettings).Name));
            if (settings != null)
            {
                var propertiesWithGetters = settings.GetType()
                    .GetProperties()
                    .Where(pi => pi.GetGetMethod() != null);
                foreach (var p in propertiesWithGetters)
                {
                    ReportAction(string.Format(PropertyReport, p.Name, p.GetValue(settings)));
                }
            }
            else
            {
                ReportAction(string.Format(NullSettingsReport));
            }
            return settings;
        }

        #endregion
    }
}