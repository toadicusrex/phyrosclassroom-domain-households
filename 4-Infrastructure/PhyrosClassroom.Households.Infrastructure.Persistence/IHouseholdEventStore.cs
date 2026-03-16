using PhyrosClassroom.Households.Models;

namespace PhyrosClassroom.Households.Infrastructure.Persistence;

public interface IHouseholdEventStore
{
    Task AppendAsync(Guid householdId, IReadOnlyList<HouseholdEventRecord> events, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<HouseholdEventRecord>> GetByIdAsync(Guid householdId, CancellationToken cancellationToken = default);
}
