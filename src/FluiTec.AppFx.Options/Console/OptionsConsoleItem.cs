using System;
using System.Collections.Generic;
using System.Linq;
using FluiTec.AppFx.Console.ConsoleItems;
using Spectre.Console;

namespace FluiTec.AppFx.Options.Console
{
    /// <summary>   The options console item. </summary>
    public class OptionsConsoleItem : SelectConsoleItem
    {
        /// <summary>   Constructor. </summary>
        /// <param name="module">   The module. </param>
        /// <param name="item">     The item. </param>
        public OptionsConsoleItem(OptionsConsoleModule module, KeyValuePair<string, string> item) : base(
            item.Key.Contains(':') ? item.Key[(item.Key.LastIndexOf(':') + 1)..] : item.Key)
        {
            Module = module;

            var (key, _) = item;
            Key = key;
        }

        /// <summary>   Gets the module. </summary>
        /// <value> The module. </value>
        public OptionsConsoleModule Module { get; }

        /// <summary>   Gets or sets the key. </summary>
        /// <value> The key. </value>
        public string Key { get; set; }

        /// <summary>   Gets or sets the value. </summary>
        /// <value> The value. </value>
        public string Value
        {
            get => Module.GetSettingValue(Key);
            set
            {
                if (value != Value)
                {
                    // TODO: use Presenter.ErrorText
                    AnsiConsole.MarkupLine($"The {Presenter.HighlightText("new value")} is \"{value}\"");
                    AnsiConsole.MarkupLine(Module.EditSetting(Key, value)
                        ? $"The {Presenter.HighlightText("new value")} was saved"
                        : $"[red]Missing {nameof(Module.SaveEnabledProvider)}. Changes could not be saved.[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine(
                        $"The new value {Presenter.HighlightText("equals")} the current value - no changes saved");
                }
            }
        }

        /// <summary>   Displays this. </summary>
        /// <param name="parent">   The parent. </param>
        public override void Display(IConsoleItem parent)
        {
            base.Display(parent);

            // if item contains elements - SelectConsoleItem (parent) already did it's thing
            if (Items.Any()) return;

            // if item doesnt contain element - let the user view/edit the value and after doing so - return control
            Presenter.PresentHeader($"View/Edit {{{Name}}} - current value:");
            AnsiConsole.WriteLine(Value);
            AnsiConsole.Render(new Rule().RuleStyle(Presenter.Style.DefaultTextStyle).LeftAligned());

            if (AnsiConsole.Confirm("Edit value?"))
                Value = AnsiConsole.Ask<string>(
                    $"Please enter a {Presenter.HighlightText("new value")}:{Environment.NewLine}");

            Parent.Display(null);
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
            return new[] {new AddOptionConsoleItem(Module)}.Concat(base.CreateDefaultItems());
        }
    }
}