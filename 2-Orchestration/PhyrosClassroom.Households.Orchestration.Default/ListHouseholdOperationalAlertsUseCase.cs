using PhyrosClassroom.Households.Infrastructure.Persistence;
using PhyrosClassroom.Households.Models;

namespace PhyrosClassroom.Households.Orchestration.Default;

public sealed class ListHouseholdOperationalAlertsUseCase(IHouseholdReadModelStore readModelStore) : IListHouseholdOperationalAlertsUseCase
{
    public async Task<IReadOnlyList<HouseholdOperationalAlert>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var households = await readModelStore.ListAsync(cancellationToken);
        return households
            .SelectMany(BuildAlerts)
            .OrderByDescending(alert => alert.Severity, StringComparer.OrdinalIgnoreCase)
            .ThenBy(alert => alert.HouseholdName, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static IEnumerable<HouseholdOperationalAlert> BuildAlerts(HouseholdReadModel household)
    {
        var updatedAtUtc = household.UpdatedAtUtc ?? household.RegisteredAtUtc;
        var contacts = household.Contacts ?? [];
        var students = household.Students ?? [];

        if (!contacts.Any(contact => contact.IsPrimaryContact))
        {
            yield return new HouseholdOperationalAlert(
                household.HouseholdId,
                household.SubjectId,
                household.HouseholdName,
                "MissingPrimaryContact",
                "High",
                "Household does not have a designated primary contact.",
                updatedAtUtc);
        }

        if (contacts.Any(contact => contact.IsPrimaryContact && (string.IsNullOrWhiteSpace(contact.Email) || string.IsNullOrWhiteSpace(contact.Phone))))
        {
            yield return new HouseholdOperationalAlert(
                household.HouseholdId,
                household.SubjectId,
                household.HouseholdName,
                "PrimaryContactDetails",
                "Medium",
                "Primary contact is missing an email address or phone number.",
                updatedAtUtc);
        }

        if (contacts.Any(contact => contact.WantsPortalAccess && string.IsNullOrWhiteSpace(contact.Email)))
        {
            yield return new HouseholdOperationalAlert(
                household.HouseholdId,
                household.SubjectId,
                household.HouseholdName,
                "PortalAccessGap",
                "Medium",
                "A contact wants portal access but does not have an email address.",
                updatedAtUtc);
        }

        if (students.Any(student => !student.StudentId.HasValue))
        {
            yield return new HouseholdOperationalAlert(
                household.HouseholdId,
                household.SubjectId,
                household.HouseholdName,
                "UnlinkedStudent",
                "Medium",
                "One or more household students are not linked to a provisioned student record yet.",
                updatedAtUtc);
        }

        if (!students.Any())
        {
            yield return new HouseholdOperationalAlert(
                household.HouseholdId,
                household.SubjectId,
                household.HouseholdName,
                "NoStudents",
                "Low",
                "Household currently has no linked students.",
                updatedAtUtc);
        }
    }
}
