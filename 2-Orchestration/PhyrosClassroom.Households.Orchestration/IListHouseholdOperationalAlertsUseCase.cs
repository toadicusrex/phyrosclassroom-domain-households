using PhyrosClassroom.Households.Models;

namespace PhyrosClassroom.Households.Orchestration;

public interface IListHouseholdOperationalAlertsUseCase
{
    Task<IReadOnlyList<HouseholdOperationalAlert>> ExecuteAsync(CancellationToken cancellationToken = default);
}
