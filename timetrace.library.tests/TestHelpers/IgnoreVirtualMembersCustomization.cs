using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;

namespace timetrace.library.tests.TestHelpers;
public class IgnoreVirtualMembersCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customizations.Add(new IgnoreVirtualMembers());
    }
    private class IgnoreVirtualMembers : ISpecimenBuilder
    {
        public object? Create(object request, ISpecimenContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var propertyInfo = request as PropertyInfo;
            if (propertyInfo == null)
            {
                return new NoSpecimen();
            }

            if (propertyInfo.GetMethod != null && propertyInfo.GetMethod.IsVirtual)
            {
                return null;
            }

            return new NoSpecimen();
        }
    } 
}
