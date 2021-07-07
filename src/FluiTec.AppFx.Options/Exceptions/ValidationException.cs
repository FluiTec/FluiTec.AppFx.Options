using System;
using FluentValidation.Results;
using FluiTec.AppFx.Options.Helpers;

namespace FluiTec.AppFx.Options.Exceptions
{
    public class ValidationException : Exception
    {
        /// <summary>   Constructor. </summary>
        /// <param name="result">   The result. </param>
        /// <param name="settingType">The settingType</param>
        /// <param name="message">  The message. </param>
        public ValidationException(ValidationResult result, Type settingType, string message) : base(message)
        {
            ValidationResult = result;
            SettingType = settingType;
        }

        /// <summary>   Gets the validation result. </summary>
        /// <value> The validation result. </value>
        public ValidationResult ValidationResult { get; }

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
                        ", SettingType={0}" +
                        ", ValidationResult={1}",
                        SettingType,
                        ValidationResult);
                });
        }
    }
}