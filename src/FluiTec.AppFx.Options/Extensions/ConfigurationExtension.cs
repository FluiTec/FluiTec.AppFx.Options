﻿using System;
using FluiTec.AppFx.Options.Exceptions;
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
        /// <param name="required">Determines of the setting is required.</param>
        /// <returns>The configured settings.</returns>
        /// <exception cref="System.ArgumentNullException">services or manager</exception>
        /// <exception cref="MissingSettingException">Thrown when specified setting is not configured.</exception>
        public static TSettings Configure<TSettings>(this IServiceCollection services, ConfigurationManager manager, bool required = false)
            where TSettings : class, new()
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (manager == null) throw new ArgumentNullException(nameof(manager));

            var settings = manager.Configure<TSettings>(services);
            if (required && settings == null)
                throw new MissingSettingException(typeof(TSettings));
            return settings;
        }

        /// <summary>Configures the specified manager.</summary>
        /// <typeparam name="TSettings">The type of the settings.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="manager">The manager.</param>
        /// <param name="configurationKey">The configuration key.</param>
        /// <param name="required">Determines of the setting is required.</param>
        /// <returns>The configured settings.</returns>
        /// <exception cref="System.ArgumentNullException">services or manager</exception>
        /// <exception cref="MissingSettingException">Thrown when specified setting is not configured.</exception>
        public static TSettings Configure<TSettings>(this IServiceCollection services, ConfigurationManager manager, string configurationKey, bool required = false)
            where TSettings : class, new()
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (manager == null) throw new ArgumentNullException(nameof(manager));

            var settings = manager.Configure<TSettings>(services, configurationKey);
            if (required && settings == null)
                throw new MissingSettingException(typeof(TSettings));
            return settings;
        }
    }
}
