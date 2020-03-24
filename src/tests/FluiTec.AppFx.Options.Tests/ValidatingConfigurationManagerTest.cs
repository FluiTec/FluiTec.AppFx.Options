using System;
using System.Collections.Generic;
using System.Linq;
using FluiTec.AppFx.Options.Exceptions;
using FluiTec.AppFx.Options.Managers;
using FluiTec.AppFx.Options.Tests.ConfigurationOptions;
using FluiTec.AppFx.Options.Tests.ConfigurationOptions.Validators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluiTec.AppFx.Options.Tests
{
    [TestClass]
    public class ValidatingConfigurationManagerTest : ConfigurationManagerTest
    {
        protected override ConfigurationManager GetManager(IConfigurationRoot configuration)
        {
            return new ValidatingConfigurationManager(configuration);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void ThrowOnInvalidSetting()
        {
            var services = new ServiceCollection();
            const string stringSetting = "";
            var builder = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>($"{nameof(OptionWithDefaultKey)}:{nameof(OptionWithDefaultKey.StringSetting)}", stringSetting)
                });
            var config = builder.Build();
            var manager = GetManager(config) as ValidatingConfigurationManager;
            Assert.IsNotNull(manager);
            
            manager.ConfigureValidator(new OptionWithDefaultKeyValidator());
            var unused = services.Configure<OptionWithDefaultKey>(manager);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))] // aggregate-exception instead of ValidationException
        public void ThrowsOnChangedInvalidSetting()
        {
            var services = new ServiceCollection();
            const string stringSetting = "test";
            var builder = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>($"{nameof(OptionWithDefaultKey)}:{nameof(OptionWithDefaultKey.StringSetting)}", stringSetting)
                });
            var config = builder.Build();
            var manager = GetManager(config) as ValidatingConfigurationManager;
            Assert.IsNotNull(manager);

            manager.ConfigureValidator(new OptionWithDefaultKeyValidator());
            var unused = services.Configure<OptionWithDefaultKey>(manager);

            var sp = services.BuildServiceProvider();
            sp.UseSettingsValidator(manager);

            config.Providers.Single().Set($"{nameof(OptionWithDefaultKey)}:{nameof(OptionWithDefaultKey.StringSetting)}", string.Empty);
            config.Reload();
        }
    }
}
