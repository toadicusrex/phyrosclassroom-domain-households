using PhyrosClassroom.Households.Infrastructure.Persistence;
using PhyrosClassroom.Households.Models;

namespace PhyrosClassroom.Households.Orchestration.Default;

public sealed class ReviewHouseholdPortalAccessUseCase(
    IHouseholdEventStore eventStore,
    IHouseholdReadModelStore readModelStore,
    IHouseholdHydratedModelCache hydratedModelCache) : IReviewHouseholdPortalAccessUseCase
{
    public async Task<HouseholdAggregate> ExecuteAsync(ReviewHouseholdPortalAccessRequest request, CancellationToken cancellationToken = default)
    {
        var household = await hydratedModelCache.GetAsync(request.HouseholdId, cancellationToken);
        if (household is null)
        {
            var eventHistory = await eventStore.GetByIdAsync(request.HouseholdId, cancellationToken);
            household = HouseholdAggregate.Rehydrate(eventHistory);
        }

        if (household is null)
        {
            throw new InvalidOperationException("Household was not found.");
        }

        var normalizedDecision = NormalizeDecision(request.Decision);
        var contacts = (household.Contacts ?? [])
            .Select(contact => Matches(contact, request)
                ? contact with
                {
                    WantsPortalAccess = !string.Equals(normalizedDecision, "Declined", StringComparison.OrdinalIgnoreCase),
                    PortalAccessStatus = normalizedDecision,
                    PortalAccessReviewedAtUtc = DateTimeOffset.UtcNow,
                    PortalAccessReviewedByUserId = NormalizeOptional(request.ReviewedByUserId) ?? "system",
                    PortalAccessReviewNotes = NormalizeOptional(request.ReviewNotes),
                }
                : contact)
            .ToArray();

        if (!contacts.Any(contact => Matches(contact, request)))
        {
            throw new InvalidOperationException("Portal access contact was not found.");
        }

        var notes = (household.Notes ?? [])
            .Append(new HouseholdRecordNote(
                "PortalAccessReview",
                $"{request.FullName.Trim()} portal access {normalizedDecision.ToLowerInvariant()}{(string.IsNullOrWhiteSpace(request.ReviewNotes) ? string.Empty : $": {request.ReviewNotes!.Trim()}")}",
                DateTimeOffset.UtcNow,
                NormalizeOptional(request.ReviewedByUserId) ?? "system"))
            .ToArray();

        household.UpdateProfile(
            household.HouseholdName,
            household.WantsEmailNotifications,
            household.WantsSmsNotifications,
            household.OnboardingStatus,
            contacts,
            household.Students ?? [],
            household.SourceRegistrationId,
            household.SourceRegistrationStatus,
            household.SourceRegistrationUpdatedAtUtc,
            household.BirthDate,
            household.GradeLevel,
            household.PrimaryGuardianName,
            household.PrimaryGuardianEmail,
            household.HasMedicalAlert,
            household.MedicalNotes,
            household.HasIep,
            household.Documents ?? [],
            notes,
            DateTimeOffset.UtcNow);

        await eventStore.AppendAsync(household.HouseholdId, [household.Events[^1]], cancellationToken);
        await readModelStore.UpsertAsync(household.ToReadModel(), cancellationToken);
        await hydratedModelCache.SetAsync(household, cancellationToken);

        return household;
    }

    private static bool Matches(HouseholdContact contact, ReviewHouseholdPortalAccessRequest request)
    {
        if (!string.Equals(contact.FullName.Trim(), request.FullName.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return true;
        }

        return string.Equals(contact.Email?.Trim(), request.Email.Trim(), StringComparison.OrdinalIgnoreCase)
            || string.Equals(contact.AlternateEmail?.Trim(), request.Email.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeDecision(string? decision)
    {
        if (string.Equals(decision?.Trim(), "Declined", StringComparison.OrdinalIgnoreCase))
        {
            return "Declined";
        }

        if (string.Equals(decision?.Trim(), "Pending", StringComparison.OrdinalIgnoreCase))
        {
            return "Pending";
        }

        return "Approved";
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
