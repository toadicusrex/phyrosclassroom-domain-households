using PhyrosClassroom.Households.Models;

namespace PhyrosClassroom.Households.Orchestration;

public interface IUpdateHouseholdProfileUseCase
{
    Task<HouseholdAggregate> ExecuteAsync(UpdateHouseholdProfileRequest request, CancellationToken cancellationToken = default);
}
