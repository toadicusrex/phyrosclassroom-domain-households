namespace PhyrosClassroom.Households.Presentation.QueryApi.Tests;

public sealed class QueryApiCompilationGuardTests
{
    [Fact]
    public void PresentationAssembly_IsLoadable()
    {
        var assembly = typeof(PhyrosClassroom.Households.Presentation.QueryApi.QueryApiEndpointRouteBuilderExtensions).Assembly;

        Assert.NotNull(assembly);
    }
}
