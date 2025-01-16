using TagCloud.TextHandlers;

namespace TagCloudTests.TestData;

public class FileTextHandlerTestData
{
    public static IEnumerable<TestCaseData> Data
    {
        get
        {
            yield return new TestCaseData(
                    "два\nодин\nдва\nа\nтри\nтри\nтри",
                    new Dictionary<string, int>()
                    {
                        { "один", 1 }, { "два", 2 }, { "три", 3 }
                    }
                )
                .SetName("ShouldExcludeConjAndNotExcludeOtherWords");
            yield return new TestCaseData("и а или", new Dictionary<string, int>())
                .SetName("ShouldExcludeAllConj");
            yield return new TestCaseData(
                    "КАПС\nКаПС\nКАПс",
                    new Dictionary<string, int>()
                    {
                        { "капс", 3 }
                    }
                )
                .SetName("ShouldTransformToLowerCase");
        }
    }
}