using PhyrosClassroom.Households.Models;

namespace PhyrosClassroom.Households.Orchestration;

public interface IGetHouseholdAtPointInTimeUseCase
{
    Task<HouseholdAggregate?> ExecuteAsync(Guid householdId, DateTimeOffset pointInTimeUtc, CancellationToken cancellationToken = default);
}
