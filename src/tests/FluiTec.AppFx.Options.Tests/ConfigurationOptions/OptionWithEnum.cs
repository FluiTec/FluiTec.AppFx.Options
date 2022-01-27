namespace FluiTec.AppFx.Options.Tests.ConfigurationOptions;

public class OptionWithEnum
{
    public enum TestEnum
    {
        Test1,

        // ReSharper disable once UnusedMember.Global
        Test2
    }

    public TestEnum EnumTest { get; set; }
}