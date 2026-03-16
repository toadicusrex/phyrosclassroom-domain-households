using PhyrosClassroom.Households.Engines;
using PhyrosClassroom.Households.Infrastructure.Persistence;
using PhyrosClassroom.Households.Models;

namespace PhyrosClassroom.Households.Orchestration.Default;

public sealed class RegisterHouseholdUseCase(
    IHouseholdCodeGenerator codeGenerator,
    IHouseholdEventStore eventStore,
    IHouseholdReadModelStore readModelStore,
    IHouseholdHydratedModelCache hydratedModelCache) : IRegisterHouseholdUseCase
{
    private const string IdentitySubjectSourceSystem = "identity-subject";

    public async Task<HouseholdAggregate> ExecuteAsync(RegisterHouseholdRequest request, CancellationToken cancellationToken = default)
    {
        var sourceSystem = string.IsNullOrWhiteSpace(request.SourceSystem) ? IdentitySubjectSourceSystem : request.SourceSystem.Trim();
        var sourceReference = string.IsNullOrWhiteSpace(request.SourceReference) ? request.SubjectId.Trim() : request.SourceReference.Trim();

        if (sourceSystem is not null && sourceReference is not null)
        {
            var existingReadModel = await readModelStore.GetBySourceReferenceAsync(sourceSystem, sourceReference, cancellationToken);
            if (existingReadModel is not null)
            {
                var cachedHousehold = await hydratedModelCache.GetAsync(existingReadModel.HouseholdId, cancellationToken);
                if (cachedHousehold is not null)
                {
                    return cachedHousehold;
                }

                var eventHistory = await eventStore.GetByIdAsync(existingReadModel.HouseholdId, cancellationToken);
                var hydratedHousehold = HouseholdAggregate.Rehydrate(eventHistory);
                if (hydratedHousehold is not null)
                {
                    await hydratedModelCache.SetAsync(hydratedHousehold, cancellationToken);
                    return hydratedHousehold;
                }
            }
        }

        var householdId = Guid.NewGuid();
        var occurredUtc = DateTimeOffset.UtcNow;
        var householdCode = codeGenerator.GenerateCode(request.GivenName, request.FamilyName, occurredUtc);

        var household = HouseholdAggregate.Register(
            householdId,
            householdCode,
            request.GivenName,
            request.FamilyName,
            occurredUtc,
            request.SubjectId,
            request.HouseholdName,
            request.WantsEmailNotifications,
            request.WantsSmsNotifications,
            request.OnboardingStatus,
            request.Contacts,
            request.Students,
            request.SourceRegistrationId,
            request.SourceRegistrationStatus,
            request.SourceRegistrationUpdatedAtUtc,
            sourceSystem,
            sourceReference);

        await eventStore.AppendAsync(household.HouseholdId, household.Events, cancellationToken);
        await readModelStore.UpsertAsync(household.ToReadModel(), cancellationToken);
        await hydratedModelCache.SetAsync(household, cancellationToken);

        return household;
    }
}
