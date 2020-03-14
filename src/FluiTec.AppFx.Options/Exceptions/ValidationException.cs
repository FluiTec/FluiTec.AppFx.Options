using System;
using FluentValidation.Results;

namespace FluiTec.AppFx.Options.Exceptions
{
    public class ValidationException : Exception
    {
        /// <summary>   Gets the validation result. </summary>
        /// <value> The validation result. </value>
        public ValidationResult ValidationResult { get; }

        /// <summary>Gets the type of the setting.</summary>
        /// <value>The type of the setting.</value>
        public Type SettingType { get; }

        /// <summary>   Constructor. </summary>
        /// 
        /// <param name="result">   The result. </param>
        /// <param name="settingType">The settingType</param>
        /// <param name="message">  The message. </param>
        public ValidationException(ValidationResult result, Type settingType, string message) : base(message)
        {
            ValidationResult = result;
            SettingType = settingType;
        }
	}
}
