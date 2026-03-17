using PhyrosClassroom.Households.Models;

namespace PhyrosClassroom.Households.Orchestration;

public interface ISearchHouseholdContactsUseCase
{
    Task<IReadOnlyList<HouseholdContactSearchResult>> ExecuteAsync(string query, CancellationToken cancellationToken = default);
}
