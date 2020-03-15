using System;
using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ValidationException = FluiTec.AppFx.Options.Exceptions.ValidationException;

namespace FluiTec.AppFx.Options.Managers
{
    /// <summary>ConfigurationManager providing validation of settings.</summary>
    /// <seealso cref="FluiTec.AppFx.Options.Managers.ConfigurationManager" />
    public class ValidatingConfigurationManager : ConfigurationManager
    {
        public readonly Dictionary<Type, IValidator> Validators = new Dictionary<Type, IValidator>();

        /// <summary>Initializes a new instance of the <see cref="ValidatingConfigurationManager"/> class.</summary>
        /// <param name="configuration">The configuration.</param>
        public ValidatingConfigurationManager(IConfigurationRoot configuration) : base(configuration)
        {
        }

        /// <summary>Extracts the settings.</summary>
        /// <typeparam name="TSettings">The type of the settings.</typeparam>
        /// <param name="configurationKey"></param>
        /// <returns>The settings.</returns>
        /// <exception cref="ValidationException">Validation for setting failed.</exception>
        /// <remarks>
        /// Will get the required section as indicated by <see cref="configurationKey"/>
        /// and bind a new instance of <see cref="TSettings"/> to the section
        /// returning that instance. (no cache involved)
        /// This method should only be used for direct inspection of certain
        /// options, since it won't register any settings to any kind of
        /// ServiceCollection.
        /// </remarks>
        public override TSettings ExtractSettings<TSettings>(string configurationKey)
        {
            var setting =  base.ExtractSettings<TSettings>(configurationKey);

            if (!Validators.ContainsKey(typeof(TSettings))) return setting;
            var result = Validators[typeof(TSettings)].Validate(setting);
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
                var result = Validate(Validators[o.GetType()], o);
                if (!result.IsValid)
                    throw new ValidationException(result, o.GetType(), "Changed variable caused ValidationFailure.");
            });
        }

        /// <summary>Validates the specified validator.</summary>
        /// <param name="validator">The validator.</param>
        /// <param name="setting">The setting.</param>
        /// <returns></returns>
        protected virtual ValidationResult Validate(IValidator validator, object setting)
        {
            return validator.Validate(setting);
        }
    }
}
