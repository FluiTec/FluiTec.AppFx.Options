using System.Collections.Generic;
using System.Linq;
using FluiTec.AppFx.Console.ConsoleItems;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.DependencyInjection;

namespace FluiTec.AppFx.Options.Console
{
    /// <summary>
    ///     The options console module.
    /// </summary>
    public class OptionsConsoleModule : ModuleConsoleItem
    {
        /// <summary>
        ///     (Immutable) the configuration values.
        /// </summary>
        private IOrderedEnumerable<KeyValuePair<string, string>> _configValues;

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

        public IOrderedEnumerable<KeyValuePair<string, string>> ConfigValues
        {
            get => _configValues;
            set
            {
                _configValues = value;
                RecreateItems();
            }
        }

        /// <summary>   Initializes this. </summary>
        protected override void Initialize()
        {
            ConfigurationRoot = Application.HostServices.GetRequiredService<IConfigurationRoot>();
            var providers = ConfigurationRoot.Providers
                .Where(p => p.GetType() != typeof(EnvironmentVariablesConfigurationProvider));

            ConfigValues = new ConfigurationRoot(providers.ToList()).AsEnumerable().OrderBy(v => v.Key);
        }

        /// <summary>
        ///     Recreate items.
        /// </summary>
        private void RecreateItems()
        {
            Items.Clear();
            foreach (var val in _configValues)
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
                if (parent.Items.SingleOrDefault(item => item.Name == parentName) is not SelectConsoleItem nParent)
                {
                    nParent = new OptionsConsoleItem(this, new KeyValuePair<string, string>(split[i], null));
                    parent.Items.Add(nParent);
                }

                parent = nParent;
            }

            return parent;
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
        public bool EditSetting(string key, string value)
        {
            if (ConfigValues.Any(cv => cv.Key == key))
            {
                SaveEnabledProvider?.Set(key, value);
            }
            else
            {
                SaveEnabledProvider?.Set(key, value);
                Initialize();
            }

            return SaveEnabledProvider != null;
        }

        /// <summary>   Edit setting. </summary>
        /// <param name="item"> The item. </param>
        public bool EditSetting(KeyValuePair<string, string> item)
        {
            return EditSetting(item.Key, item.Value);
        }

        /// <summary>
        ///     Enumerates create default items in this collection.
        /// </summary>
        /// <returns>
        ///     An enumerator that allows foreach to be used to process create default items in this
        ///     collection.
        /// </returns>
        protected override IEnumerable<IConsoleItem> CreateDefaultItems()
        {
            var def = base.CreateDefaultItems();
            return new[] {new AddOptionConsoleItem(this)}.Concat(def);
        }
    }
}