namespace PhyrosClassroom.Households.Presentation.CommandApi.Tests;

public sealed class CommandApiCompilationGuardTests
{
    [Fact]
    public void PresentationAssembly_IsLoadable()
    {
        var assembly = typeof(PhyrosClassroom.Households.Presentation.CommandApi.CommandApiEndpointRouteBuilderExtensions).Assembly;

        Assert.NotNull(assembly);
    }
}
