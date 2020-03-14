using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluiTec.AppFx.Options.Attributes;
using Microsoft.Extensions.Configuration;

namespace FluiTec.AppFx.Options.Managers
{
    /// <summary>A simple ConfigurationManager.</summary>
    /// <remarks>
    /// Provides:
    /// a) Naming of ConfigurationKeys (cached)
    /// b) Extraction of Options from an IConfigurationRoot
    /// </remarks>
    public class ConfigurationManager
    {
        #region Fields

        protected readonly Dictionary<Type, string> ConfigurationKeys = new Dictionary<Type, string>();

        protected readonly IConfigurationRoot Configuration;

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="ConfigurationManager"/> class.</summary>
        /// <param name="configuration">The configuration.</param>
        /// <exception cref="System.ArgumentNullException">configuration</exception>
        public ConfigurationManager(IConfigurationRoot configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
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
        /// </remarks>
        public virtual string GetKeyByType(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            // return cached key if available
            if (ConfigurationKeys.ContainsKey(type)) return ConfigurationKeys[type];

            // try to find ConfigurationKeyAttribute
            var attribute =
                type.GetTypeInfo().GetCustomAttributes(typeof(ConfigurationKeyAttribute)).SingleOrDefault() as
                    ConfigurationKeyAttribute;

            // add key to cache
            ConfigurationKeys.Add(type, attribute != null ? attribute.Name : type.Name);

            // return key
            return ConfigurationKeys[type];
        }

        #endregion

        #region Extraction

        /// <summary>Extracts the settings.</summary>
        /// <typeparam name="TSettings">The type of the settings.</typeparam>
        /// <returns>The settings.</returns>
        /// <remarks>
        /// Will get the required section as indicated by <see cref="GetKeyByType"/>
        /// and bind a new instance of <see cref="TSettings"/> to the section
        /// returning that instance. (no cache involved)
        /// This method should only be used for direct inspection of certain
        /// options, since it won't register any settings to any kind of
        /// ServiceCollection.
        /// </remarks>
        public virtual TSettings ExtractSettings<TSettings>() where TSettings : class, new()
        {
            var key = GetKeyByType(typeof(TSettings));
            var section = Configuration.GetSection(key);
            var settings = section.Get<TSettings>();
            return settings;
        }

        #endregion
    }
}