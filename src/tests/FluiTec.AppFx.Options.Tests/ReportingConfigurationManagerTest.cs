using System;
using System.Collections.Generic;
using System.Linq;
using FluiTec.AppFx.Options.Managers;
using FluiTec.AppFx.Options.Tests.ConfigurationOptions;
using FluiTec.AppFx.Options.Tests.ConfigurationOptions.Validators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluiTec.AppFx.Options.Tests
{
    [TestClass]
    public class ReportingConfigurationManagerTest : ValidatingConfigurationManagerTest
    {
        private readonly List<string> _reportEntries = new List<string>();

        protected override ConfigurationManager GetManager(IConfigurationRoot configuration)
        {
            return new ReportingConfigurationManager(configuration, ReportAction);
        }

        private void ReportAction(string report)
        {
            _reportEntries.Add(report);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsOnMissingReportAction()
        {
            _reportEntries.Clear();
            var unused = new ReportingConfigurationManager(GetEmptyConfiguration(), null);
        }

        [TestMethod]
        public void CanReportKeyNaming()
        {
            _reportEntries.Clear();
            var config = GetEmptyConfiguration();
            var manager = GetManager(config) as ReportingConfigurationManager;
            Assert.IsNotNull(manager);
            var key = manager.GetKeyByType(typeof(OptionWithDefaultKey));
            Assert.IsTrue(_reportEntries.Contains(string.Format(manager.ConfigurationKeyReport, key)));
        }

        [TestMethod]
        public void CanReportEmptySettings()
        {
            _reportEntries.Clear();
            var config = GetEmptyConfiguration();
            var manager = GetManager(config) as ReportingConfigurationManager;
            Assert.IsNotNull(manager);

            var unused = manager.ExtractSettings<OptionWithDefaultKey>();
            Assert.IsTrue(_reportEntries.Contains(string.Format(manager.ExtractSettingsReport, typeof(OptionWithDefaultKey).Name)));
        }

        [TestMethod]
        public void CanReportSimpleSettings()
        {
            _reportEntries.Clear();
            const string stringSetting = "test";
            var builder = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>($"{nameof(OptionWithDefaultKey)}:{nameof(OptionWithDefaultKey.StringSetting)}", stringSetting),
                });
            var config = builder.Build();
            var manager = GetManager(config) as ReportingConfigurationManager;
            Assert.IsNotNull(manager);

            var unused = manager.ExtractSettings<OptionWithDefaultKey>();
            Assert.IsTrue(_reportEntries.Contains(string.Format(manager.PropertyReport, nameof(OptionWithDefaultKey.StringSetting), stringSetting)));
        }

        [TestMethod]
        public void CanReportInheritedSettings()
        {
            _reportEntries.Clear();
            const string stringSetting = "test";
            const string stringSetting2 = "test2";

            var builder = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>($"{nameof(InheritedOption)}:{nameof(InheritedOption.StringSetting)}", stringSetting),
                    new KeyValuePair<string, string>($"{nameof(InheritedOption)}:{nameof(InheritedOption.StringSetting2)}", stringSetting2)
                });
            var config = builder.Build();
            var manager = GetManager(config) as ReportingConfigurationManager;
            Assert.IsNotNull(manager);

            var unused = manager.ExtractSettings<InheritedOption>();
            Assert.IsTrue(_reportEntries.Contains(string.Format(manager.PropertyReport, nameof(InheritedOption.StringSetting), stringSetting)));
            Assert.IsTrue(_reportEntries.Contains(string.Format(manager.PropertyReport, nameof(InheritedOption.StringSetting2), stringSetting2)));
        }

        [TestMethod]
        public void CanReportNestedSettings()
        {
            _reportEntries.Clear();
            const string stringSetting = "test";
            var builder = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>($"{nameof(NestingOption)}:{nameof(NestingOption.Test)}:{nameof(OptionWithDefaultKey.StringSetting)}", stringSetting)
                });
            var config = builder.Build();
            var manager = GetManager(config) as ReportingConfigurationManager;
            Assert.IsNotNull(manager);

            var unused = manager.ExtractSettings<NestingOption>();
            Assert.IsTrue(_reportEntries.Contains(string.Format("-" + manager.PropertyReport, nameof(InheritedOption.StringSetting), stringSetting)));
        }

        [TestMethod]
        public void RedactsSimpleSecrets()
        {
            _reportEntries.Clear();
            const string stringSetting = "test";
            var builder = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>($"{nameof(SecretOption)}:{nameof(SecretOption.StringSetting)}", stringSetting),
                });
            var config = builder.Build();
            var manager = GetManager(config) as ReportingConfigurationManager;
            Assert.IsNotNull(manager);

            var unused = manager.ExtractSettings<SecretOption>();
            Assert.IsTrue(_reportEntries.Contains(string.Format(manager.PropertyReport, nameof(SecretOption.StringSetting), manager.RedactedValueReplacement)));
        }

        [TestMethod]
        public void RedactsInheritedSecrets()
        {
            _reportEntries.Clear();
            const string stringSetting = "test";
            var builder = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>($"{nameof(InheritedSecretOption)}:{nameof(InheritedSecretOption.StringSetting)}", stringSetting),
                });
            var config = builder.Build();
            var manager = GetManager(config) as ReportingConfigurationManager;
            Assert.IsNotNull(manager);

            var unused = manager.ExtractSettings<InheritedSecretOption>();
            Assert.IsTrue(_reportEntries.Contains(string.Format(manager.PropertyReport, nameof(InheritedSecretOption.StringSetting), manager.RedactedValueReplacement)));
        }
        
        [TestMethod]
        public void TestServiceCollectionReporting()
        {
            _reportEntries.Clear();

            var services = new ServiceCollection();
            const string stringSetting = "test";
            var builder = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>($"{nameof(OptionWithDefaultKey)}:{nameof(OptionWithDefaultKey.StringSetting)}", stringSetting),
                });
            var config = builder.Build();
            var manager = GetManager(config);
            var settings = manager.Configure<OptionWithDefaultKey>(services);

            var serviceProvider = services.BuildServiceProvider();
            Assert.AreEqual(settings.StringSetting, serviceProvider.GetService<IOptions<OptionWithDefaultKey>>().Value.StringSetting);
        }

        [TestMethod]
        public void ReportsOnChangedSetting()
        {
            _reportEntries.Clear();

            var services = new ServiceCollection();
            const string stringSetting = "test";
            var builder = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>($"{nameof(OptionWithDefaultKey)}:{nameof(OptionWithDefaultKey.StringSetting)}", stringSetting),
                });
            var config = builder.Build();
            var manager = GetManager(config) as ReportingConfigurationManager;
            Assert.IsNotNull(manager);

            manager.ConfigureValidator(new OptionWithDefaultKeyValidator());
            var unused = services.Configure<OptionWithDefaultKey>(manager);

            var sp = services.BuildServiceProvider();
            sp.UseSettingsValidator(manager);

            config.Providers.Single().Set($"{nameof(OptionWithDefaultKey)}:{nameof(OptionWithDefaultKey.StringSetting)}", "test2");
            config.Reload();

            Assert.IsTrue(_reportEntries.Contains(string.Format(manager.SettingsChangedReport, nameof(OptionWithDefaultKey))));
        }
    }
}