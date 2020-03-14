using FluiTec.AppFx.Options.Attributes;

namespace FluiTec.AppFx.Options.ConsoleSample.Configuration
{
    [ConfigurationKey("AppSettings")]
    public class ApplicationSettings
    {
        public string Name { get; set; }
    }
}
