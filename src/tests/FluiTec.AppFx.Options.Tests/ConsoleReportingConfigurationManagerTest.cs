using FluiTec.AppFx.Options.Managers;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluiTec.AppFx.Options.Tests
{
    [TestClass]
    public class ConsoleReportingConfigurationManagerTest : ValidatingConfigurationManagerTest
    {
        protected override ConfigurationManager GetManager(IConfigurationRoot configuration)
        {
            return new ConsoleReportingConfigurationManager(configuration);
        }
    }
}