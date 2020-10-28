# README #

## AppFx ##

#### Options ####
* FluiTec.AppFx.Options : simplify the handling of json-configuration-files

### Build ###
[![Build status](https://ci.appveyor.com/api/projects/status/h2910uwkgg8034ba?svg=true)](https://ci.appveyor.com/project/IInvocation/fluitec-appfx-options)

### Nuget ###
[![Nuget version](https://img.shields.io/nuget/v/FluiTec.AppFx.Options.svg)](https://www.nuget.org/packages/FluiTec.AppFx.Options/)

### Using it ###
1. install FluiTec.AppFx.Options
2. setup appsettings
3. setup dependency injection
4. create a class containing options (and possibly a validator), example:
```
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
```
5. configure dependency injection
```
// config is typeof(IConfigurationRoot)
// create a ConfigurationManager for the Configuration
var manager = new ConsoleReportingConfigurationManager(config);
// services is typeof(IServiceCollection)
// register the application-settings
services.Configure<ApplicationSettings>(manager);
// configure a validator
manager.ConfigureValidator(new ApplicationSettingsValidator());
```
