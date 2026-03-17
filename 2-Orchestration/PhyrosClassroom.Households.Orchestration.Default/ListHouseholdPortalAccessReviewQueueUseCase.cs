using PhyrosClassroom.Households.Infrastructure.Persistence;
using PhyrosClassroom.Households.Models;

namespace PhyrosClassroom.Households.Orchestration.Default;

public sealed class ListHouseholdPortalAccessReviewQueueUseCase(IHouseholdReadModelStore readModelStore) : IListHouseholdPortalAccessReviewQueueUseCase
{
    public async Task<IReadOnlyList<HouseholdPortalAccessReviewItem>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var households = await readModelStore.ListAsync(cancellationToken);

        return households
            .SelectMany(household => (household.Contacts ?? [])
                .Where(contact => contact.WantsPortalAccess)
                .Select(contact =>
                {
                    var inviteEmail = !string.IsNullOrWhiteSpace(contact.Email)
                        ? contact.Email.Trim()
                        : !string.IsNullOrWhiteSpace(contact.AlternateEmail)
                            ? contact.AlternateEmail.Trim()
                            : null;
                    var readyForInvite = !string.IsNullOrWhiteSpace(inviteEmail);

                    return new HouseholdPortalAccessReviewItem(
                        household.HouseholdId,
                        household.SubjectId,
                        household.HouseholdName,
                        contact.FullName,
                        contact.RelationshipToChildren,
                        inviteEmail,
                        contact.AlternateEmail,
                        contact.MobilePhone,
                        contact.IsPrimaryContact,
                        contact.IsEmergencyContact,
                        readyForInvite,
                        readyForInvite ? "Ready" : "MissingEmail",
                        household.UpdatedAtUtc ?? household.RegisteredAtUtc);
                }))
            .OrderByDescending(item => item.ReadyForInvite)
            .ThenBy(item => item.HouseholdName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(item => item.FullName, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
