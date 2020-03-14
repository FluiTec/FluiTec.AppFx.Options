using System;
using System.Collections.Generic;
using FluiTec.AppFx.Options.Managers;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluiTec.AppFx.Options.Tests
{
    [TestClass]
    public class ReportingConfigurationManagerTest : ConfigurationManagerTest
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
            var unused = new ReportingConfigurationManager(GetEmptyConfiguration(), null);
        }

        [TestMethod]
        public void CanReportKeyNaming()
        {
            var config = GetEmptyConfiguration();
            var manager = GetManager(config) as ReportingConfigurationManager;
            Assert.IsNotNull(manager);
            var key = manager.GetKeyByType(typeof(OptionWithDefaultKey));
            Assert.IsTrue(_reportEntries.Contains(string.Format(manager.ConfigurationKeyReport, key)));
        }
    }
}