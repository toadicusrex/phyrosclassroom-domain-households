using PhyrosClassroom.Households.Engines;
using PhyrosClassroom.Households.Infrastructure.Persistence;
using PhyrosClassroom.Households.Models;
using PhyrosClassroom.Households.Orchestration;
using PhyrosClassroom.Households.Orchestration.Default;

namespace PhyrosClassroom.Households.Orchestration.Default.Tests;

public sealed class RegisterHouseholdUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_PersistsHouseholdAndHydratesCache()
    {
        var eventStore = new FakeHouseholdEventStore();
        var readModelStore = new FakeHouseholdReadModelStore();
        var hydratedModelCache = new FakeHouseholdHydratedModelCache();
        var useCase = new RegisterHouseholdUseCase(
            new StubHouseholdCodeGenerator(),
            eventStore,
            readModelStore,
            hydratedModelCache);

        var household = await useCase.ExecuteAsync(
            new RegisterHouseholdRequest("Alice", "Bennett"));

        var storedEvents = await eventStore.GetByIdAsync(household.HouseholdId);
        var storedReadModel = await readModelStore.GetByIdAsync(household.HouseholdId);
        var cachedHousehold = await hydratedModelCache.GetAsync(household.HouseholdId);

        Assert.Single(storedEvents);
        Assert.NotNull(storedReadModel);
        Assert.NotNull(cachedHousehold);
        Assert.Equal("AB-REFERENCE", household.HouseholdCode);
        Assert.Equal("Alice", storedReadModel!.GivenName);
    }

    [Fact]
    public async Task ExecuteAsync_RehydratesFromCacheBeforeReadingEventStore()
    {
        var cachedHousehold = HouseholdAggregate.Register(
            Guid.NewGuid(),
            "AB-REFERENCE",
            "Alice",
            "Bennett",
            new DateTimeOffset(2026, 2, 28, 8, 0, 0, TimeSpan.Zero));

        var eventStore = new FakeHouseholdEventStore();
        var hydratedModelCache = new FakeHouseholdHydratedModelCache();
        await hydratedModelCache.SetAsync(cachedHousehold);

        var useCase = new GetHouseholdByIdUseCase(eventStore, hydratedModelCache);

        var household = await useCase.ExecuteAsync(cachedHousehold.HouseholdId);

        Assert.NotNull(household);
        Assert.Equal(cachedHousehold.HouseholdId, household!.HouseholdId);
        Assert.Equal(0, eventStore.ReadCount);
    }

    private sealed class StubHouseholdCodeGenerator : IHouseholdCodeGenerator
    {
        public string GenerateCode(string givenName, string familyName, DateTimeOffset occurredUtc)
        {
            return "AB-REFERENCE";
        }
    }

    private sealed class FakeHouseholdEventStore : IHouseholdEventStore
    {
        private readonly Dictionary<Guid, List<HouseholdEventRecord>> _events = [];

        public int ReadCount { get; private set; }

        public Task AppendAsync(Guid householdId, IReadOnlyList<HouseholdEventRecord> events, CancellationToken cancellationToken = default)
        {
            if (!_events.TryGetValue(householdId, out var eventList))
            {
                eventList = [];
                _events[householdId] = eventList;
            }

            eventList.AddRange(events);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<HouseholdEventRecord>> GetByIdAsync(Guid householdId, CancellationToken cancellationToken = default)
        {
            ReadCount++;
            return Task.FromResult<IReadOnlyList<HouseholdEventRecord>>(
                _events.TryGetValue(householdId, out var eventList) ? eventList : []);
        }
    }

    private sealed class FakeHouseholdReadModelStore : IHouseholdReadModelStore
    {
        private readonly Dictionary<Guid, HouseholdReadModel> _households = [];

        public Task UpsertAsync(HouseholdReadModel household, CancellationToken cancellationToken = default)
        {
            _households[household.HouseholdId] = household;
            return Task.CompletedTask;
        }

        public Task<HouseholdReadModel?> GetByIdAsync(Guid householdId, CancellationToken cancellationToken = default)
        {
            _households.TryGetValue(householdId, out var household);
            return Task.FromResult(household);
        }

        public Task<HouseholdReadModel?> GetBySourceReferenceAsync(string sourceSystem, string sourceReference, CancellationToken cancellationToken = default)
        {
            var household = _households.Values.FirstOrDefault(item =>
                string.Equals(item.SourceSystem, sourceSystem, StringComparison.Ordinal) &&
                string.Equals(item.SourceReference, sourceReference, StringComparison.Ordinal));
            return Task.FromResult(household);
        }

        public Task<IReadOnlyList<HouseholdReadModel>> ListAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<HouseholdReadModel>>(_households.Values.ToList());
        }
    }

    private sealed class FakeHouseholdHydratedModelCache : IHouseholdHydratedModelCache
    {
        private readonly Dictionary<Guid, HouseholdAggregate> _households = [];

        public Task<HouseholdAggregate?> GetAsync(Guid householdId, CancellationToken cancellationToken = default)
        {
            _households.TryGetValue(householdId, out var household);
            return Task.FromResult(household);
        }

        public Task SetAsync(HouseholdAggregate household, CancellationToken cancellationToken = default)
        {
            _households[household.HouseholdId] = household;
            return Task.CompletedTask;
        }
    }
}
