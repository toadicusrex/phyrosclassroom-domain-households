using PhyrosClassroom.Households.Infrastructure.Persistence;
using PhyrosClassroom.Households.Models;

namespace PhyrosClassroom.Households.Orchestration.Default;

public sealed class ListHouseholdsUseCase(IHouseholdReadModelStore readModelStore) : IListHouseholdsUseCase
{
    public Task<IReadOnlyList<HouseholdReadModel>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return readModelStore.ListAsync(cancellationToken);
    }
}
