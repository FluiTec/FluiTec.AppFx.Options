using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;
using FluentValidation;
using FluentValidation.Results;
using FluiTec.AppFx.Options.Attributes;
using Microsoft.Extensions.Configuration;

namespace FluiTec.AppFx.Options.Managers
{
    /// <summary>A reporting ConfigurationManager.</summary>
    /// <seealso cref="FluiTec.AppFx.Options.Managers.ConfigurationManager" />
    public class ReportingConfigurationManager : ValidatingConfigurationManager
    {
        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="ReportingConfigurationManager" /> class.</summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="reportAction">The report action.</param>
        /// <exception cref="System.ArgumentNullException">reportAction</exception>
        public ReportingConfigurationManager(IConfigurationRoot configuration, Action<string> reportAction) : base(
            configuration)
        {
            ReportAction = reportAction ?? throw new ArgumentNullException(nameof(reportAction));

            // setup reports
            ConfigurationKeyReport = "[ConfigurationKey]: '{0}'";
            ExtractSettingsReport = "[ConfigurationSettings of '{0}':]";
            NullSettingsReport = "-> No settings provided.";
            PropertyReport = "-> '{0}' = '{1}'";
            RedactedValueReplacement = "** REDACTED **";
            SettingsChangedReport = "[ConfigurationSettings of '{0}' changed:]";
        }

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

        /// <summary>Gets or sets the redacted value replacement.</summary>
        /// <value>The redacted value replacement.</value>
        public string RedactedValueReplacement { get; set; }

        /// <summary>Gets or sets the settings changed report.</summary>
        /// <value>The settings changed report.</value>
        public string SettingsChangedReport { get; set; }

        #region Fields

        private static readonly Type[] UninspectedTypes =
        {
            typeof(string),
            typeof(bool),
            typeof(char),
            typeof(double),
            typeof(decimal),
            typeof(byte),
            typeof(float),
            typeof(int),
            typeof(long),
            typeof(short),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan),
            typeof(Enum),
            typeof(CultureInfo),
            typeof(Guid)
        };

        protected readonly Action<string> ReportAction;

        #endregion

        #region Naming

        /// <summary>Gets the ConfigurationKey using the given type.</summary>
        /// <param name="type">The type to inspect.</param>
        /// <exception cref="System.ArgumentNullException">type</exception>
        /// <returns>The ConfigurationKey</returns>
        /// <remarks>
        ///     Behavior:
        ///     a) Will check internal cache if the the type was ever checked (and return accordingly)
        ///     b) Will inspect the ConfigurationKeyAttribute (cache and return accordingly)
        ///     c) Will use nameof(type) (cache and return accordingly)
        ///     d) Will report the ConfigurationKey
        /// </remarks>
        public override string GetKeyByType(Type type)
        {
            var key = base.GetKeyByType(type);
            ReportAction(string.Format(ConfigurationKeyReport, key));
            return key;
        }

        /// <summary>Extracts the settings.</summary>
        /// <typeparam name="TSettings">The type of the settings.</typeparam>
        /// <param name="configurationKey"></param>
        /// <param name="required"></param>
        /// <returns>The settings.</returns>
        /// <exception cref="ArgumentException">configurationKey empty</exception>
        /// <exception cref="ArgumentNullException">configurationKey</exception>
        /// <remarks>
        ///     Will get the required section as indicated by <see cref="GetKeyByType" />
        ///     and bind a new instance of <see cref="TSettings" /> to the section
        ///     returning that instance. (no cache involved)
        ///     This method should only be used for direct inspection of certain
        ///     options, since it won't register any settings to any kind of
        ///     ServiceCollection. Will also report extracted setting.
        ///     Report will redact properties marked with <see cref="ConfigurationSecretAttribute" />.
        /// </remarks>
        public override TSettings ExtractSettings<TSettings>(string configurationKey, bool required = false)
        {
            var settings = base.ExtractSettings<TSettings>(configurationKey, required);
            ReportAction(string.Format(ExtractSettingsReport, typeof(TSettings).Name));
            ReportSettingProperties(settings);
            return settings;
        }

        /// <summary>Validates the specified validator.</summary>
        /// <param name="validator">The validator.</param>
        /// <param name="setting">The setting.</param>
        /// <param name="objectType"></param>
        /// <returns>The result of the Validation</returns>
        protected override ValidationResult Validate(IValidator validator, object setting, Type objectType)
        {
            var result = base.Validate(validator, setting, objectType);
            ReportAction(string.Format(SettingsChangedReport, objectType.Name));
            ReportSettingProperties(setting);
            return result;
        }

        /// <summary>Reports the setting properties.</summary>
        /// <param name="settings">The settings.</param>
        /// <param name="indentation">The indentation to use</param>
        protected virtual void ReportSettingProperties(object settings, int indentation = 0)
        {
            if (settings != null)
            {
                if (settings is IEnumerable enumerable)
                {
                    // report all elements in enumerable
                    ReportEnumerableSettingProperties(settings, enumerable, indentation);
                }
                else
                {
                    // patch up indentation
                    var indent = "";
                    for (var i = 0; i < indentation; i++)
                        indent += "-";

                    // actually report
                    if (settings.GetType().IsEnum)
                    {
                        // ignore here (is already reported)
                    }
                    else
                    {
                        ReportDefaultSettingProperty(settings, indent, indentation);
                    }
                }
            }
            else
            {
                ReportAction(string.Format(NullSettingsReport));
            }
        }

        /// <summary>   Reports enumerable setting properties. </summary>
        /// <param name="settings">     The settings. </param>
        /// <param name="enumerable">   The enumerable. </param>
        /// <param name="indentation">  (Optional) The indentation to use. </param>
        protected virtual void ReportEnumerableSettingProperties(object settings, IEnumerable enumerable,
            int indentation = 0)
        {
            foreach (var setting in enumerable)
                ReportSettingProperties(setting, indentation + 1);
        }

        /// <summary>   Reports enum setting property. </summary>
        /// <param name="settings">     The settings. </param>
        /// <param name="indent">       The indent. </param>
        protected virtual void ReportEnumSettingProperty(object settings, string indent)
        {
            ReportAction(string.Format($"{indent}{PropertyReport}", "Enum",
                Enum.GetName(settings.GetType(), settings)));
        }

        /// <summary>   Reports default setting property. </summary>
        /// <param name="settings">     The settings. </param>
        /// <param name="indent">       The indent. </param>
        /// <param name="indentation">  The indentation to use. </param>
        protected virtual void ReportDefaultSettingProperty(object settings, string indent, int indentation)
        {
            var propertiesWithGetters = settings.GetType()
                .GetProperties()
                .Where(pi =>
                    pi.GetGetMethod() != null && pi.GetMethod.IsPublic && pi.GetMethod.IsStatic == false);
            foreach (var p in propertiesWithGetters)
            {
                var isSecret = p.GetCustomAttributes(true)
                                   .SingleOrDefault(a => a.GetType() == typeof(ConfigurationSecretAttribute)) !=
                               null;
                try
                {
                    var value = p.GetValue(settings);
                    ReportAction(string.Format($"{indent}{PropertyReport}", p.Name,
                        isSecret ? RedactedValueReplacement : value));

                    if (value == null) continue;
                    var valueType = value.GetType();
                    if (!UninspectedTypes.Contains(valueType) && !isSecret)
                        ReportSettingProperties(value, indentation + 1);
                }
                catch (TargetParameterCountException)
                {
                    // ignore
                }
            }
        }

        #endregion
    }
}