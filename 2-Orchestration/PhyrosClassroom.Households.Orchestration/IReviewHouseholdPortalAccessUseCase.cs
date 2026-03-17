using PhyrosClassroom.Households.Models;

namespace PhyrosClassroom.Households.Orchestration;

public interface IReviewHouseholdPortalAccessUseCase
{
    Task<HouseholdAggregate> ExecuteAsync(ReviewHouseholdPortalAccessRequest request, CancellationToken cancellationToken = default);
}
