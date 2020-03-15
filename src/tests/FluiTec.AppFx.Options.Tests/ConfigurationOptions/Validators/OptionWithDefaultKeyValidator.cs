using FluentValidation;

namespace FluiTec.AppFx.Options.Tests.ConfigurationOptions.Validators
{
    public class OptionWithDefaultKeyValidator : AbstractValidator<OptionWithDefaultKey>
    {
        public OptionWithDefaultKeyValidator()
        {
            RuleFor(setting => setting.StringSetting).NotEmpty().Length(1, 15);
        }
    }
}