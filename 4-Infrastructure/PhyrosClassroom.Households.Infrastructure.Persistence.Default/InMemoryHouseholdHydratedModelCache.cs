using PhyrosClassroom.Households.Infrastructure.Persistence;
using PhyrosClassroom.Households.Models;

namespace PhyrosClassroom.Households.Infrastructure.Persistence.Default;

public sealed class InMemoryHouseholdHydratedModelCache : IHouseholdHydratedModelCache
{
    private readonly Dictionary<Guid, HouseholdAggregate> _households = [];
    private readonly object _gate = new();

    public Task<HouseholdAggregate?> GetAsync(Guid householdId, CancellationToken cancellationToken = default)
    {
        lock (_gate)
        {
            _households.TryGetValue(householdId, out var household);
            return Task.FromResult(household);
        }
    }

    public Task SetAsync(HouseholdAggregate household, CancellationToken cancellationToken = default)
    {
        lock (_gate)
        {
            _households[household.HouseholdId] = household;
        }

        return Task.CompletedTask;
    }
}
