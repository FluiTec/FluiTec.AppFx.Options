using System;
using System.Collections.Generic;
using System.Linq;
using FluiTec.AppFx.Console.ConsoleItems;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FluiTec.AppFx.Options.Console
{
    /// <summary>   The options console module. </summary>
    public class OptionsConsoleModule : ModuleConsoleItem
    {
        /// <summary>   Constructor. </summary>
        /// <param name="saveEnabledProvider">  The save enabled provider. </param>
        public OptionsConsoleModule(IConfigurationProvider saveEnabledProvider) : base("Options")
        {
            SaveEnabledProvider = saveEnabledProvider;
        }

        /// <summary>   Gets the save enabled provider. </summary>
        /// <value> The save enabled provider. </value>
        public IConfigurationProvider SaveEnabledProvider { get; }

        /// <summary>   Gets or sets the configuration root. </summary>
        /// <value> The configuration root. </value>
        private IConfigurationRoot ConfigurationRoot { get; set; }

        /// <summary>   Initializes this. </summary>
        protected override void Initialize()
        {
            ConfigurationRoot = Application.HostServices.GetRequiredService<IConfigurationRoot>();
            var providers = ConfigurationRoot.Providers
                .Where(p => p.GetType() != typeof(EnvironmentVariablesConfigurationProvider));

            var configValues = new ConfigurationRoot(providers.ToList()).AsEnumerable().OrderBy(v => v.Key);
            foreach (var val in configValues)
            {
                var parent = val.Key.Contains(':') ? FindParent(val) : this;
                parent.Items.Add(new OptionsConsoleItem(this, val));
            }
        }

        /// <summary>   Finds the parent of this item. </summary>
        /// <param name="entry">    The entry. </param>
        /// <returns>   The found parent. </returns>
        protected SelectConsoleItem FindParent(KeyValuePair<string, string> entry)
        {
            var split = entry.Key.Split(':').ToList();

            SelectConsoleItem parent = this;
            for (var i = 0; i < split.Count - 1; i++)
            {
                var parentName = split[i];
                var nParent = parent.Items.SingleOrDefault(item => item.Name == parentName) as SelectConsoleItem;
                if (nParent == null)
                {
                    nParent = new OptionsConsoleItem(this, new KeyValuePair<string, string>(split[i], null));
                    parent.Items.Add(nParent);
                }

                parent = nParent;
            }

            return parent;
        }

        /// <summary>   Finds the configured option types in this collection. </summary>
        /// <returns>
        ///     An enumerator that allows foreach to be used to process the configured option types in
        ///     this collection.
        /// </returns>
        private IEnumerable<Type> FindConfiguredOptionTypes()
        {
            if (Application.HostServices.GetRequiredService(typeof(IServiceCollection)) is not IServiceCollection
                services) return Enumerable.Empty<Type>();

            return services
                .Select(s => s.ServiceType)
                .Where(s => s.IsGenericType &&
                            typeof(IConfigureOptions<>).IsAssignableFrom(s.GetGenericTypeDefinition()))
                .Select(s => s.GenericTypeArguments.Single())
                .ToList();
        }

        /// <summary>   Gets setting value. </summary>
        /// <param name="key">  The key. </param>
        /// <returns>   The setting value. </returns>
        public string GetSettingValue(string key)
        {
            return ConfigurationRoot[key];
        }

        /// <summary>   Edit setting. </summary>
        /// <param name="key">      The key. </param>
        /// <param name="value">    The value. </param>
        public void EditSetting(string key, string value)
        {
            SaveEnabledProvider?.Set(key, value);
        }

        /// <summary>   Edit setting. </summary>
        /// <param name="item"> The item. </param>
        public void EditSetting(KeyValuePair<string, string> item)
        {
            EditSetting(item.Key, item.Value);
        }
    }
}