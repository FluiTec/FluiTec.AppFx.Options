using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using FluiTec.AppFx.Console.ConsoleItems;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

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

        /// <summary>
        /// Gets or sets the configuration values.
        /// </summary>
        ///
        /// <value>
        /// The configuration values.
        /// </value>
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

        /// <summary>
        /// Configure command.
        /// </summary>
        ///
        /// <returns>
        /// A System.CommandLine.Command.
        /// </returns>
        public override Command ConfigureCommand()
        {
            var cmd = new Command("--options", "Configuration of the application. (Key/Value store)");

            var editCmd = new Command("--edit", "Edit configuration of the application.");
            editCmd.AddOption(new Option<string>("--key", "Key of the configuration-entry.") {IsRequired = true});
            editCmd.AddOption(new Option<string>("--value", "(New) Value of the configuration-entry.") {IsRequired = false});
            editCmd.Handler = CommandHandler.Create(new System.Func<string, string, int>(ProcessEdit));

            var addCmd = new Command("--add", "Add an entry to configuration of the application.");
            addCmd.AddOption(new Option<string>("--key", "Key of the configuration-entry.") {IsRequired = true});
            addCmd.AddOption(new Option<string>("--value", "(New) Value of the configuration-entry.") {IsRequired = false});
            addCmd.Handler = CommandHandler.Create(new System.Func<string, string, int>(ProcessEdit));

            var viewCmd = new Command("--read", "Read the value of a configuration-key,");
            viewCmd.AddOption(new Option<string>("--key", "Key of the configuration-entry.") {IsRequired = true});
            viewCmd.Handler = CommandHandler.Create(new System.Func<string, int>(ProcessRead));

            cmd.AddCommand(editCmd);
            cmd.AddCommand(addCmd);
            cmd.AddCommand(viewCmd);

            return cmd;
        }

        /// <summary>
        /// Process the edit.
        /// </summary>
        ///
        /// <param name="key">  The key. </param>
        /// <param name="val">  The value. </param>
        private int ProcessEdit(string key, string val)
        {
            if (EditSetting(key, val))
            {
                AnsiConsole.MarkupLine($"The value of key '{Presenter.HighlightText(key)}' was set to '{Presenter.HighlightText(val)}'");
                return (int)ExitCode.Success;
            }

            Presenter.ErrorText($"Missing {nameof(SaveEnabledProvider)}. Changes could not be saved.");
            return (int)ExitCode.Error;
        }

        /// <summary>
        /// Process the read described by key.
        /// </summary>
        ///
        /// <param name="key">  The key. </param>
        private int ProcessRead(string key)
        {
            var (s, value) = ConfigValues.SingleOrDefault(cv => cv.Key == key);
            if (s != null)
            {
                System.Console.WriteLine(value);
                return (int) ExitCode.Success;
            }

            System.Console.WriteLine("Non-existent config-key. Could not find value.");
            return (int) ExitCode.Error;
        }

        /// <summary>
        /// Values that represent exit codes.
        /// </summary>
        public enum ExitCode
        {
            /// <summary>
            /// An enum constant representing the success option.
            /// </summary>
            Success = 0,
            /// <summary>
            /// An enum constant representing the error option.
            /// </summary>
            Error = 1
        }
    }
}