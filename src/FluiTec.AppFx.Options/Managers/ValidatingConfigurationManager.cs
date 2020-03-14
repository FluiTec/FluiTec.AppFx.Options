using System;
using System.Collections.Generic;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using ValidationException = FluiTec.AppFx.Options.Exceptions.ValidationException;

namespace FluiTec.AppFx.Options.Managers
{
    public class ValidatingConfigurationManager : ConfigurationManager
    {
        public readonly Dictionary<Type, IValidator> Validators = new Dictionary<Type, IValidator>();

        public ValidatingConfigurationManager(IConfigurationRoot configuration) : base(configuration)
        {
        }

        public override TSettings ExtractSettings<TSettings>(string configurationKey)
        {
            var setting =  base.ExtractSettings<TSettings>(configurationKey);

            if (!Validators.ContainsKey(typeof(TSettings))) return setting;
            var result = Validators[typeof(TSettings)].Validate(setting);
            if (!result.IsValid)
                throw new ValidationException(result, typeof(TSettings), "Validation for setting failed.");

            return setting;
        }
    }
}
