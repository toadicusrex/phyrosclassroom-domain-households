using PhyrosClassroom.Households.Models;

namespace PhyrosClassroom.Households.Orchestration;

public interface IGetHouseholdReadModelBySubjectIdUseCase
{
    Task<HouseholdReadModel?> ExecuteAsync(string subjectId, CancellationToken cancellationToken = default);
}
