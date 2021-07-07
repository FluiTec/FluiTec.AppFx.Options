namespace FluiTec.AppFx.Options.Tests.ConfigurationOptions
{
    public class OptionWithEnum
    {
        public enum TestEnum
        {
            Test1,
            Test2
        }

        public TestEnum EnumTest { get; set; }
    }
}