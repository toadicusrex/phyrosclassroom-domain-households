using PhyrosClassroom.Households.Infrastructure.Persistence;
using PhyrosClassroom.Households.Models;

namespace PhyrosClassroom.Households.Orchestration.Default;

public sealed class GetHouseholdEventHistoryUseCase(IHouseholdEventStore eventStore) : IGetHouseholdEventHistoryUseCase
{
    public Task<IReadOnlyList<HouseholdEventRecord>> ExecuteAsync(Guid householdId, CancellationToken cancellationToken = default)
    {
        return eventStore.GetByIdAsync(householdId, cancellationToken);
    }
}
