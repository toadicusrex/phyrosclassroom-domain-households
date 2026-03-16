using PhyrosClassroom.Households.Models;

namespace PhyrosClassroom.Households.Infrastructure.Persistence;

public interface IHouseholdHydratedModelCache
{
    Task<HouseholdAggregate?> GetAsync(Guid householdId, CancellationToken cancellationToken = default);

    Task SetAsync(HouseholdAggregate household, CancellationToken cancellationToken = default);
}
