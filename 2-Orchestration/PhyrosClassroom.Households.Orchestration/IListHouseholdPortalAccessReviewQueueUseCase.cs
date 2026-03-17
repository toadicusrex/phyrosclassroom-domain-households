using PhyrosClassroom.Households.Models;

namespace PhyrosClassroom.Households.Orchestration;

public interface IListHouseholdPortalAccessReviewQueueUseCase
{
    Task<IReadOnlyList<HouseholdPortalAccessReviewItem>> ExecuteAsync(CancellationToken cancellationToken = default);
}
