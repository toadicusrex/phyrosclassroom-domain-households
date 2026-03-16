using PhyrosClassroom.Households.Infrastructure.Persistence;
using PhyrosClassroom.Households.Models;

namespace PhyrosClassroom.Households.Orchestration.Default;

public sealed class GetHouseholdReadModelByIdUseCase(IHouseholdReadModelStore readModelStore) : IGetHouseholdReadModelByIdUseCase
{
    public Task<HouseholdReadModel?> ExecuteAsync(Guid householdId, CancellationToken cancellationToken = default)
    {
        return readModelStore.GetByIdAsync(householdId, cancellationToken);
    }
}
