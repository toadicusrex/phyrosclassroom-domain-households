using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PhyrosClassroom.Households.Orchestration;

namespace PhyrosClassroom.Households.Presentation.QueryApi;

public static class QueryApiEndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapHouseholdQueryApi(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/query/households");

        group.MapGet("/", async (
            IListHouseholdsUseCase useCase,
            CancellationToken cancellationToken) =>
        {
            var households = await useCase.ExecuteAsync(cancellationToken);
            return Results.Ok(households);
        });

        group.MapGet("/contacts/search", async (
            string q,
            ISearchHouseholdContactsUseCase useCase,
            CancellationToken cancellationToken) =>
        {
            var results = await useCase.ExecuteAsync(q, cancellationToken);
            return Results.Ok(results);
        });

        group.MapGet("/{householdId:guid}", async (
            Guid householdId,
            IGetHouseholdReadModelByIdUseCase useCase,
            CancellationToken cancellationToken) =>
        {
            var household = await useCase.ExecuteAsync(householdId, cancellationToken);
            return household is null ? Results.NotFound() : Results.Ok(household);
        });

        group.MapGet("/by-subject/{subjectId}", async (
            string subjectId,
            IGetHouseholdReadModelBySubjectIdUseCase useCase,
            CancellationToken cancellationToken) =>
        {
            var household = await useCase.ExecuteAsync(subjectId, cancellationToken);
            return household is null ? Results.NotFound() : Results.Ok(household);
        });

        return endpoints;
    }
}
