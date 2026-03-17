using PhyrosClassroom.Households.Infrastructure.Persistence;
using PhyrosClassroom.Households.Models;

namespace PhyrosClassroom.Households.Orchestration.Default;

public sealed class SearchHouseholdContactsUseCase(IHouseholdReadModelStore readModelStore) : ISearchHouseholdContactsUseCase
{
    public async Task<IReadOnlyList<HouseholdContactSearchResult>> ExecuteAsync(string query, CancellationToken cancellationToken = default)
    {
        var normalizedQuery = query?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedQuery))
        {
            return [];
        }

        var households = await readModelStore.ListAsync(cancellationToken);
        return households
            .SelectMany(household => (household.Contacts ?? [])
                .Where(contact =>
                    contact.FullName.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                    contact.Email.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                    contact.Phone.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase))
                .Select(contact => new HouseholdContactSearchResult(
                    household.HouseholdId,
                    household.SubjectId,
                    household.HouseholdName,
                    contact.FullName,
                    contact.RelationshipToChildren,
                    contact.Email,
                    contact.Phone,
                    contact.IsPrimaryContact,
                    household.Students?.Count ?? 0,
                    household.UpdatedAtUtc ?? household.RegisteredAtUtc)))
            .OrderBy(result => result.HouseholdName)
            .ThenBy(result => result.FullName)
            .ToArray();
    }
}
