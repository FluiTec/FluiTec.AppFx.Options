using System;
using System.Linq;
using FluentValidation;
using FluiTec.AppFx.Options.Exceptions;
using FluiTec.AppFx.Options.Managers;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>Simple extension to simplify using the ConfigurationManager.</summary>
    public static class ConfigurationExtension
    {
        /// <summary>Uses the settings validator.</summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="manager">The manager.</param>
        /// <exception cref="ArgumentNullException">serviceProvider or manager</exception>
        public static IServiceProvider UseSettingsValidator(this IServiceProvider serviceProvider, ValidatingConfigurationManager manager)
        {
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));
            if (manager == null) throw new ArgumentNullException(nameof(manager));

            foreach (var monitor in manager.Validators.Keys
                .Select(settingsType => typeof(IOptionsMonitor<>)
                .MakeGenericType(settingsType))
                .Select(monitorType => serviceProvider.GetService(monitorType) as IOptionsMonitor<object>))
            {
                if (monitor != null)
                    manager.KeepValidating(monitor);
            }

            return serviceProvider;
        }

        /// <summary>Configures the validator.</summary>
        /// <typeparam name="TSettings">The type of the settings.</typeparam>
        /// <param name="manager">The manager.</param>
        /// <param name="validator">The validator.</param>
        public static ValidatingConfigurationManager ConfigureValidator<TSettings>(this ValidatingConfigurationManager manager, IValidator<TSettings> validator)
        {
            if (manager == null) throw new ArgumentNullException(nameof(manager));
            if (validator == null) throw new ArgumentNullException(nameof(validator));

            manager.Validators.Add(typeof(TSettings), validator);
            return manager;
        }

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
