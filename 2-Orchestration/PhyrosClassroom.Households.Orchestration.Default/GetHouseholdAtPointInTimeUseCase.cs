using PhyrosClassroom.Households.Infrastructure.Persistence;
using PhyrosClassroom.Households.Models;

namespace PhyrosClassroom.Households.Orchestration.Default;

public sealed class GetHouseholdAtPointInTimeUseCase(IHouseholdEventStore eventStore) : IGetHouseholdAtPointInTimeUseCase
{
    public async Task<HouseholdAggregate?> ExecuteAsync(
        Guid householdId,
        DateTimeOffset pointInTimeUtc,
        CancellationToken cancellationToken = default)
    {
        var eventHistory = await eventStore.GetByIdAsync(householdId, cancellationToken);
        return HouseholdAggregate.RehydrateAt(eventHistory, pointInTimeUtc);
    }
}
