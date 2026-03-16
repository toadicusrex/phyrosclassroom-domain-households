using PhyrosClassroom.Households.Models;

namespace PhyrosClassroom.Households.Infrastructure.Persistence;

public interface IHouseholdReadModelStore
{
    Task UpsertAsync(HouseholdReadModel household, CancellationToken cancellationToken = default);

    Task<HouseholdReadModel?> GetByIdAsync(Guid householdId, CancellationToken cancellationToken = default);

    Task<HouseholdReadModel?> GetBySourceReferenceAsync(string sourceSystem, string sourceReference, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<HouseholdReadModel>> ListAsync(CancellationToken cancellationToken = default);
}
