using FluentValidation;
using FluiTec.AppFx.Options.Attributes;

namespace FluiTec.AppFx.Options.ConsoleSample.Configuration
{
    [ConfigurationKey("AppSettings")]
    public class ApplicationSettings
    {
        public string Name { get; set; }
    }

    public class ApplicationSettingsValidator : AbstractValidator<ApplicationSettings>
    {
        public ApplicationSettingsValidator()
        {
            RuleFor(setting => setting.Name).NotEmpty().Length(1, 15);
        }
    }
}