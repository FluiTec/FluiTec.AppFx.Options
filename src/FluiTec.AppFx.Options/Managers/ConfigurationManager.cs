using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluiTec.AppFx.Options.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FluiTec.AppFx.Options.Managers
{
    /// <summary>A simple ConfigurationManager.</summary>
    /// <remarks>
    /// Provides:
    /// a) Naming of ConfigurationKeys (cached)
    /// b) Extraction of Options from an IConfigurationRoot
    /// c) Configuration of a ServiceCollection
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

            // add key to cache
            ConfigurationKeys.Add(type, type.GetTypeInfo().GetCustomAttributes(typeof(ConfigurationKeyAttribute)).SingleOrDefault() is ConfigurationKeyAttribute attribute ? attribute.Name : type.Name);

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
            return ExtractSettings<TSettings>(key);
        }

        /// <summary>Extracts the settings.</summary>
        /// <typeparam name="TSettings">The type of the settings.</typeparam>
        /// <param name="configurationKey"></param>
        /// <exception cref="ArgumentException">configurationKey empty</exception>
        /// <exception cref="ArgumentNullException">configurationKey</exception>
        /// <returns>The settings.</returns>
        /// <remarks>
        /// Will get the required section as indicated by <see cref="configurationKey"/>
        /// and bind a new instance of <see cref="TSettings"/> to the section
        /// returning that instance. (no cache involved)
        /// This method should only be used for direct inspection of certain
        /// options, since it won't register any settings to any kind of
        /// ServiceCollection.
        /// </remarks>
        public virtual TSettings ExtractSettings<TSettings>(string configurationKey) where TSettings : class, new()
        {
            if (configurationKey == null) throw new ArgumentNullException(nameof(configurationKey));
            if (configurationKey == string.Empty) throw new ArgumentException("Must not be empty", nameof(configurationKey));

            var section = Configuration.GetSection(configurationKey);
            var settings = section.Get<TSettings>();
            return settings;
        }

        #endregion

        #region Configuration

        /// <summary>Configures the specified services.</summary>
        /// <typeparam name="TSettings">The type of the settings.</typeparam>
        /// <param name="services">The services.</param>
        /// <returns>The extracted settings.</returns>
        /// <exception cref="System.ArgumentNullException">services or  configurationKey</exception>
        /// <exception cref="System.ArgumentException">Must not be empty - configurationKey</exception>
        public virtual TSettings Configure<TSettings>(IServiceCollection services) where TSettings : class, new()
        {
            return Configure<TSettings>(services, GetKeyByType(typeof(TSettings)));
        }

        /// <summary>Configures the specified services.</summary>
        /// <typeparam name="TSettings">The type of the settings.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="configurationKey">The configuration key.</param>
        /// <returns>The extracted settings.</returns>
        /// <exception cref="System.ArgumentNullException">services or  configurationKey</exception>
        /// <exception cref="System.ArgumentException">Must not be empty - configurationKey</exception>
        public virtual TSettings Configure<TSettings>(IServiceCollection services, string configurationKey)
            where TSettings : class, new()
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configurationKey == null) throw new ArgumentNullException(nameof(configurationKey));
            if (configurationKey == string.Empty) throw new ArgumentException("Must not be empty", nameof(configurationKey));

            // adds settings as IOptions<TSetting>, IOptionsSnapshot<TSetting>, etc.
            services.Configure<TSettings>(settings => { Configuration.GetSection(configurationKey).Bind(settings); });

            // adds settings as TSetting
            services.AddSingleton(sp => sp.GetService<IOptions<TSettings>>().Value);

            return ExtractSettings<TSettings>(configurationKey);
        }

        #endregion
    }
}