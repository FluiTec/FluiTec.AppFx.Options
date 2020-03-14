using System;
using FluiTec.AppFx.Options.Managers;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>Simple extension to simplify using the ConfigurationManager.</summary>
    public static class ConfigurationExtension
    {
        /// <summary>Configures the specified manager.</summary>
        /// <typeparam name="TSettings">The type of the settings.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="manager">The manager.</param>
        /// <returns>The configured settings.</returns>
        /// <exception cref="System.ArgumentNullException">services or manager</exception>
        public static TSettings Configure<TSettings>(this IServiceCollection services, ConfigurationManager manager)
            where TSettings : class, new()
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (manager == null) throw new ArgumentNullException(nameof(manager));

            return manager.Configure<TSettings>(services);
        }

        /// <summary>Configures the specified manager.</summary>
        /// <typeparam name="TSettings">The type of the settings.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="manager">The manager.</param>
        /// <param name="configurationKey">The configuration key.</param>
        /// <returns>The configured settings.</returns>
        /// <exception cref="System.ArgumentNullException">services or manager</exception>
        public static TSettings Configure<TSettings>(this IServiceCollection services, ConfigurationManager manager, string configurationKey)
            where TSettings : class, new()
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (manager == null) throw new ArgumentNullException(nameof(manager));

            return manager.Configure<TSettings>(services, configurationKey);
        }
    }
}
