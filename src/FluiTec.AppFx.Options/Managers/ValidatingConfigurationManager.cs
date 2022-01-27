using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ValidationException = FluiTec.AppFx.Options.Exceptions.ValidationException;

namespace FluiTec.AppFx.Options.Managers;

/// <summary>ConfigurationManager providing validation of settings.</summary>
/// <seealso cref="FluiTec.AppFx.Options.Managers.ConfigurationManager" />
public class ValidatingConfigurationManager : ConfigurationManager
{
    public readonly Dictionary<Type, IValidator> Validators = new();

    /// <summary>Initializes a new instance of the <see cref="ValidatingConfigurationManager" /> class.</summary>
    /// <param name="configuration">The configuration.</param>
    public ValidatingConfigurationManager(IConfigurationRoot configuration) : base(configuration)
    {
    }

    /// <summary>Extracts the settings.</summary>
    /// <typeparam name="TSettings">The type of the settings.</typeparam>
    /// <param name="configurationKey"></param>
    /// <param name="required"></param>
    /// <returns>The settings.</returns>
    /// <exception cref="Exceptions.ValidationException">Validation for setting failed.</exception>
    /// <remarks>
    ///     Will get the required section as indicated by <see cref="configurationKey" />
    ///     and bind a new instance of <see cref="TSettings" /> to the section
    ///     returning that instance. (no cache involved)
    ///     This method should only be used for direct inspection of certain
    ///     options, since it won't register any settings to any kind of
    ///     ServiceCollection.
    /// </remarks>
    public override TSettings ExtractSettings<TSettings>(string configurationKey, bool required = false)
    {
        var setting = base.ExtractSettings<TSettings>(configurationKey, required);
        if (setting == null) return null;

        if (!Validators.ContainsKey(typeof(TSettings))) return setting;
        var result = Validators[typeof(TSettings)].Validate(new ValidationContext<object>(setting));
        if (!result.IsValid)
            throw new ValidationException(result, typeof(TSettings), "Validation for setting failed.");

        return setting;
    }

    /// <summary>Keeps the validating.</summary>
    /// <param name="monitor">The monitor.</param>
    public void KeepValidating(IOptionsMonitor<object> monitor)
    {
        monitor?.OnChange(o =>
        {
            var settingType = monitor.GetType().GetGenericArguments().SingleOrDefault();
            if (settingType == null) return;

            var result = Validate(Validators[settingType], o, settingType);
            if (!result.IsValid)
                throw new ValidationException(result, o.GetType(),
                    "Changed variable caused ValidationFailure.");
        });
    }

    /// <summary>Validates the specified validator.</summary>
    /// <param name="validator">The validator.</param>
    /// <param name="setting">The setting.</param>
    /// <param name="objectType"></param>
    /// <returns>The result of the Validation</returns>
    protected virtual ValidationResult Validate(IValidator validator, object setting, Type objectType)
    {
        return validator.Validate(new ValidationContext<object>(setting));
    }
}