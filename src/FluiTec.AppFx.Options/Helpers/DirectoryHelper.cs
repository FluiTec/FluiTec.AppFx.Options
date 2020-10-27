using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace FluiTec.AppFx.Options.Helpers
{
    /// <summary>   A directory helper. </summary>
    public static class DirectoryHelper
    {
        /// <summary>   Gets application root. </summary>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when the requested operation is
        ///     invalid.
        /// </exception>
        /// <returns>   The application root. </returns>
        public static string GetApplicationRoot()
        {
            var exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
                var appRoot = appPathMatcher.Match(exePath ?? throw new InvalidOperationException()).Value;
                return appRoot;
            }
            else
            {
                var appPathMatcher = new Regex(@"(?<!file)\/+[\S\s]*?(?=\/+bin)");
                var appRoot = appPathMatcher.Match(exePath ?? throw new InvalidOperationException()).Value;
                return appRoot;
            }
        }
    }
}