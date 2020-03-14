using System;
using System.Collections.Generic;
using FluiTec.AppFx.Options.Managers;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluiTec.AppFx.Options.Tests
{
    [TestClass]
    public class ConfigurationManagerTest
    {
        protected IConfigurationRoot GetEmptyConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .AddInMemoryCollection(new KeyValuePair<string, string>[] { });
            return builder.Build();
        }

        protected virtual ConfigurationManager GetManager(IConfigurationRoot configuration)
        {
            return  new ConfigurationManager(configuration);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsOnMissingConfiguration()
        {
            var unused = new ConfigurationManager(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsOnMissingTypeForKey()
        {
            var config = GetEmptyConfiguration();
            var manager = GetManager(config);
            var unused = manager.GetKeyByType(null);
        }

        [TestMethod]
        public void ExtractsDefaultKey()
        {
            var config = GetEmptyConfiguration();
            var manager = GetManager(config);
            var key = manager.GetKeyByType(typeof(OptionWithDefaultKey));
            Assert.AreEqual(nameof(OptionWithDefaultKey), key);
        }

        [TestMethod]
        public void ExtractsAttributeKey()
        {
            var config = GetEmptyConfiguration();
            var manager = GetManager(config);
            var key = manager.GetKeyByType(typeof(OptionWithAttributeKey));
            Assert.AreEqual("ConfigKey", key);
        }

        [TestMethod]
        public void CanExtractEmptySettings()
        {
            var config = GetEmptyConfiguration();
            var manager = GetManager(config);
            var settings = manager.ExtractSettings<OptionWithDefaultKey>();
            Assert.IsNull(settings);
        }

        [TestMethod]
        public void CanExtractSettings()
        {
            const string stringSetting = "test";
            var builder = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>($"{nameof(OptionWithDefaultKey)}:{nameof(OptionWithDefaultKey.StringSetting)}", stringSetting),
                });
            var config = builder.Build();
            var manager = GetManager(config);
            var setting = manager.ExtractSettings<OptionWithDefaultKey>();
            Assert.AreEqual(stringSetting, setting.StringSetting);
        }
    }
}
