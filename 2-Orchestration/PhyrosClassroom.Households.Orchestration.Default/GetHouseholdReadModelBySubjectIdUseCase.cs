using PhyrosClassroom.Households.Infrastructure.Persistence;
using PhyrosClassroom.Households.Models;

namespace PhyrosClassroom.Households.Orchestration.Default;

public sealed class GetHouseholdReadModelBySubjectIdUseCase(IHouseholdReadModelStore readModelStore) : IGetHouseholdReadModelBySubjectIdUseCase
{
    private const string SourceSystem = "identity-subject";

    public Task<HouseholdReadModel?> ExecuteAsync(string subjectId, CancellationToken cancellationToken = default)
    {
        return readModelStore.GetBySourceReferenceAsync(SourceSystem, subjectId.Trim(), cancellationToken);
    }
}
