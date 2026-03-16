using PhyrosClassroom.Households.Infrastructure.Persistence;
using PhyrosClassroom.Households.Models;

namespace PhyrosClassroom.Households.Orchestration.Default;

public sealed class GetHouseholdByIdUseCase(
    IHouseholdEventStore eventStore,
    IHouseholdHydratedModelCache hydratedModelCache) : IGetHouseholdByIdUseCase
{
    public async Task<HouseholdAggregate?> ExecuteAsync(Guid householdId, CancellationToken cancellationToken = default)
    {
        var cachedHousehold = await hydratedModelCache.GetAsync(householdId, cancellationToken);
        if (cachedHousehold is not null)
        {
            return cachedHousehold;
        }

        var eventHistory = await eventStore.GetByIdAsync(householdId, cancellationToken);
        var household = HouseholdAggregate.Rehydrate(eventHistory);

        if (household is not null)
        {
            await hydratedModelCache.SetAsync(household, cancellationToken);
        }

        return household;
    }
}
