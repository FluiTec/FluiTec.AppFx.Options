using System.Collections.Generic;
using FluiTec.AppFx.Options.Exceptions;
using FluiTec.AppFx.Options.Managers;
using FluiTec.AppFx.Options.Tests.ConfigurationOptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluiTec.AppFx.Options.Tests
{
    [TestClass]
    public class ConfigurationExtensionTest
    {
        [TestMethod]
        public void TestAutoNamedOptions()
        {
            var services = new ServiceCollection();
            const string stringSetting = "test";
            var builder = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>(
                        $"{nameof(OptionWithDefaultKey)}:{nameof(OptionWithDefaultKey.StringSetting)}", stringSetting)
                });
            var config = builder.Build();
            var manager = new ReportingConfigurationManager(config, s => { });
            services.Configure<OptionWithDefaultKey>(manager);

            var serviceProvider = services.BuildServiceProvider();
            Assert.AreEqual(stringSetting,
                serviceProvider.GetService<IOptions<OptionWithDefaultKey>>().Value.StringSetting);
        }

        [TestMethod]
        public void TestManualNamedOptions()
        {
            var services = new ServiceCollection();
            const string stringSetting = "test";
            const string sectionKey = "MyKey";
            var builder = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>($"{sectionKey}:{nameof(OptionWithDefaultKey.StringSetting)}",
                        stringSetting)
                });
            var config = builder.Build();
            var manager = new ReportingConfigurationManager(config, s => { });
            services.Configure<OptionWithDefaultKey>(manager, sectionKey);

            var serviceProvider = services.BuildServiceProvider();
            Assert.AreEqual(stringSetting,
                serviceProvider.GetService<IOptions<OptionWithDefaultKey>>().Value.StringSetting);
        }

        [TestMethod]
        [ExpectedException(typeof(MissingSettingException))]
        public void ThrowsOnUnconfiguredSetting()
        {
            var services = new ServiceCollection();
            var builder = new ConfigurationBuilder();
            var config = builder.Build();
            var manager = new ReportingConfigurationManager(config, s => { });
            services.Configure<OptionWithDefaultKey>(manager, true);
        }
    }
}