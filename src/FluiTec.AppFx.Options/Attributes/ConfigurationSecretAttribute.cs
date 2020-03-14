using System;
using FluiTec.AppFx.Options.Managers;

namespace FluiTec.AppFx.Options.Attributes
{
    /// <summary>Declares a property as a secret configuration.</summary>
    /// <seealso cref="System.Attribute" />
    /// <remarks>
    /// <see cref="ReportingConfigurationManager"/> will redact properties marked as such.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigurationSecretAttribute : Attribute
    {

    }
}