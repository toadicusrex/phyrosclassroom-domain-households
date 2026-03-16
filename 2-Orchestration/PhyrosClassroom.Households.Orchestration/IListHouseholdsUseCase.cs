using PhyrosClassroom.Households.Models;

namespace PhyrosClassroom.Households.Orchestration;

public interface IListHouseholdsUseCase
{
    Task<IReadOnlyList<HouseholdReadModel>> ExecuteAsync(CancellationToken cancellationToken = default);
}
