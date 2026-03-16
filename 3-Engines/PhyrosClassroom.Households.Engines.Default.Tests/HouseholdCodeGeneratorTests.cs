using PhyrosClassroom.Households.Engines.Default;

namespace PhyrosClassroom.Households.Engines.Default.Tests;

public sealed class HouseholdCodeGeneratorTests
{
    [Fact]
    public void GenerateCode_UsesInitialsAndTimestamp()
    {
        var generator = new HouseholdCodeGenerator();

        var code = generator.GenerateCode(
            "Alice",
            "Bennett",
            new DateTimeOffset(2026, 2, 28, 8, 30, 45, TimeSpan.Zero));

        Assert.Equal("AB-20260228083045", code);
    }
}
