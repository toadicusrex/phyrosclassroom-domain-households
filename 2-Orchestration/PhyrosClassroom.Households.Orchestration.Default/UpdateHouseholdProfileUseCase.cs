using PhyrosClassroom.Households.Infrastructure.Persistence;
using PhyrosClassroom.Households.Models;

namespace PhyrosClassroom.Households.Orchestration.Default;

public sealed class UpdateHouseholdProfileUseCase(
    IHouseholdEventStore eventStore,
    IHouseholdReadModelStore readModelStore,
    IHouseholdHydratedModelCache hydratedModelCache) : IUpdateHouseholdProfileUseCase
{
    public async Task<HouseholdAggregate> ExecuteAsync(UpdateHouseholdProfileRequest request, CancellationToken cancellationToken = default)
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

        household.UpdateProfile(
            request.HouseholdName,
            request.WantsEmailNotifications,
            request.WantsSmsNotifications,
            request.OnboardingStatus,
            request.Contacts ?? [],
            request.Students ?? [],
            request.SourceRegistrationId,
            request.SourceRegistrationStatus,
            request.SourceRegistrationUpdatedAtUtc,
            request.BirthDate,
            request.GradeLevel,
            request.PrimaryGuardianName,
            request.PrimaryGuardianEmail,
            request.HasMedicalAlert,
            request.MedicalNotes,
            request.HasIep,
            request.Documents ?? [],
            request.Notes ?? [],
            DateTimeOffset.UtcNow);

        await eventStore.AppendAsync(household.HouseholdId, [household.Events[^1]], cancellationToken);
        await readModelStore.UpsertAsync(household.ToReadModel(), cancellationToken);
        await hydratedModelCache.SetAsync(household, cancellationToken);

        return household;
    }
}
