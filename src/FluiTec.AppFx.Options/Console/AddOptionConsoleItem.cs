using System;
using FluiTec.AppFx.Console.ConsoleItems;
using FluiTec.AppFx.Console.Presentation;
using Spectre.Console;

namespace FluiTec.AppFx.Options.Console;

/// <summary>
///     An add option console item.
/// </summary>
public class AddOptionConsoleItem : IConsoleItem
{
    /// <summary>
    ///     Constructor.
    /// </summary>
    /// <param name="module">   The module. </param>
    public AddOptionConsoleItem(OptionsConsoleModule module)
    {
        Module = module;
    }

    /// <summary>   Gets the module. </summary>
    /// <value> The module. </value>
    public OptionsConsoleModule Module { get; }

    /// <summary>   Gets the presenter. </summary>
    /// <value> The presenter. </value>
    protected IConsolePresenter Presenter { get; } = ConsoleApplicationSettings.Instance.Presenter;

    /// <summary>   Gets the name. </summary>
    /// <value> The name. </value>
    public string Name => "Add new option";

    /// <summary>   Gets the name of the display. </summary>
    /// <value> The name of the display. </value>
    public string DisplayName => Name;

    /// <summary>
    ///     Gets or sets the parent.
    /// </summary>
    /// <value>
    ///     The parent.
    /// </value>
    public IConsoleItem Parent { get; private set; }

    /// <summary>
    ///     Displays this.
    /// </summary>
    /// <param name="parent">   The parent. </param>
    public void Display(IConsoleItem parent)
    {
        Parent = parent.Parent;

        var key = AnsiConsole.Ask<string>(
            $"Please enter a {Presenter.HighlightText("name")}:{Environment.NewLine}");
        var val = AnsiConsole.Ask<string>(
            $"Please enter a {Presenter.HighlightText("value")}:{Environment.NewLine}");

        var editKey = parent is OptionsConsoleItem item ? $"{item.Key}:{key}" : key;

        Module.EditSetting(editKey, val);

        Parent.Display(null);
    }
}