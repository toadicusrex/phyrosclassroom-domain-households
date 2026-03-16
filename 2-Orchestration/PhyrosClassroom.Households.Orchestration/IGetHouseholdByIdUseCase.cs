using PhyrosClassroom.Households.Models;

namespace PhyrosClassroom.Households.Orchestration;

public interface IGetHouseholdByIdUseCase
{
    Task<HouseholdAggregate?> ExecuteAsync(Guid householdId, CancellationToken cancellationToken = default);
}
