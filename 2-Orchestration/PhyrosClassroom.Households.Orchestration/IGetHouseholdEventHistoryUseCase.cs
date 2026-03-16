using PhyrosClassroom.Households.Models;

namespace PhyrosClassroom.Households.Orchestration;

public interface IGetHouseholdEventHistoryUseCase
{
    Task<IReadOnlyList<HouseholdEventRecord>> ExecuteAsync(Guid householdId, CancellationToken cancellationToken = default);
}
