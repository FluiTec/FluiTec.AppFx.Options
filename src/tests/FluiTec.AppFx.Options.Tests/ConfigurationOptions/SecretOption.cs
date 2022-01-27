using FluiTec.AppFx.Options.Attributes;

namespace FluiTec.AppFx.Options.Tests.ConfigurationOptions;

public class SecretOption
{
    [ConfigurationSecret] public string StringSetting { get; set; }
}