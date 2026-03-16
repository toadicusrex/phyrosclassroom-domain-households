using PhyrosClassroom.Households.Models;

namespace PhyrosClassroom.Households.Orchestration;

public interface IRegisterHouseholdUseCase
{
    Task<HouseholdAggregate> ExecuteAsync(RegisterHouseholdRequest request, CancellationToken cancellationToken = default);
}
