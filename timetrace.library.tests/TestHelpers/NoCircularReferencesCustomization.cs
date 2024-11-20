using AutoFixture;

namespace timetrace.library.tests.TestHelpers;

public class NoCircularReferencesCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(behavior => fixture.Behaviors.Remove(behavior));
    }
}

