using System;
using FluiTec.AppFx.Options.Helpers;

namespace FluiTec.AppFx.Options.Exceptions;

/// <summary>Describes a missing configurationSetting</summary>
/// <seealso cref="System.Exception" />
public class MissingSettingException : Exception
{
    /// <summary>Initializes a new instance of the <see cref="MissingSettingException" /> class.</summary>
    /// <param name="settingType">Type of the setting.</param>
    public MissingSettingException(Type settingType)
    {
        SettingType = settingType;
    }

    /// <summary>Gets the type of the setting.</summary>
    /// <value>The type of the setting.</value>
    public Type SettingType { get; }

    /// <summary>Converts to string.</summary>
    /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
    public override string ToString()
    {
        return this.ExceptionToString(
            description =>
            {
                description.AppendFormat(
                    ", SettingType={0}",
                    SettingType);
            });
    }
}