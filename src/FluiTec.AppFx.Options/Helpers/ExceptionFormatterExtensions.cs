using System;
using System.Text;

namespace FluiTec.AppFx.Options.Helpers;

/// <summary>Extension for ExceptionFormatter.</summary>
public static class ExceptionFormatterExtensions
{
    /// <summary>Exceptions to string.</summary>
    /// <param name="ex">The ex.</param>
    /// <param name="customFieldsFormatterAction">The custom fields formatter action.</param>
    /// <returns></returns>
    public static string ExceptionToString(
        this Exception ex,
        Action<StringBuilder> customFieldsFormatterAction)
    {
        var description = new StringBuilder();
        description.AppendFormat("{0}: {1}", ex.GetType().Name, ex.Message);

        customFieldsFormatterAction?.Invoke(description);

        if (ex.InnerException != null)
        {
            description.AppendFormat(" ---> {0}", ex.InnerException);
            description.AppendFormat(
                "{0}   --- End of inner exception stack trace ---{0}",
                Environment.NewLine);
        }

        description.Append(ex.StackTrace);

        return description.ToString();
    }
}