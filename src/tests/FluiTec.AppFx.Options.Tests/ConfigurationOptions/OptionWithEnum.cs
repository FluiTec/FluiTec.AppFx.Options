namespace FluiTec.AppFx.Options.Tests.ConfigurationOptions
{
    public class OptionWithEnum
    {
        public TestEnum EnumTest { get; set; }

        public enum TestEnum
        {
            Test1,
            Test2
        }
    }
}