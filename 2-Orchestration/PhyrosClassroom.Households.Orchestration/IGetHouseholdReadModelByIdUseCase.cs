using PhyrosClassroom.Households.Models;

namespace PhyrosClassroom.Households.Orchestration;

public interface IGetHouseholdReadModelByIdUseCase
{
    Task<HouseholdReadModel?> ExecuteAsync(Guid householdId, CancellationToken cancellationToken = default);
}
