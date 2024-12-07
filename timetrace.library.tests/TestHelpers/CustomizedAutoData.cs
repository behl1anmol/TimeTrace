using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit4;

namespace timetrace.library.tests.TestHelpers;

[AttributeUsage(AttributeTargets.Method)]
public class CustomizedAutoData : AutoDataAttribute
{
    public CustomizedAutoData() : base(CreateFixture)
    {
    }
    private static IFixture CreateFixture()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization() { ConfigureMembers = true, GenerateDelegates = true });
        fixture.Customize(new NoCircularReferencesCustomization());
        fixture.Customize(new IgnoreVirtualMembersCustomization());

        // fixture.Customize<ConfigurationSetting>(c => c.Without(cs => cs.ConfigurationSettingDetails));
        return fixture;
    }
}
