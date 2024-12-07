namespace timetrace.library.tests.TestHelpers;

/// <summary>
/// This class will accept a category and set it within category attribute
/// </summary>
public class TestCategoryAttribute(string category) : CategoryAttribute(category);
