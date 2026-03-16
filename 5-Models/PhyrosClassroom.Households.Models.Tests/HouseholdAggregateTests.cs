using PhyrosClassroom.Households.Models;

namespace PhyrosClassroom.Households.Models.Tests;

public sealed class HouseholdAggregateTests
{
    [Fact]
    public void Register_CreatesInitialEventAndReadModel()
    {
        var householdId = Guid.NewGuid();
        var occurredUtc = new DateTimeOffset(2026, 2, 28, 8, 0, 0, TimeSpan.Zero);

        var aggregate = HouseholdAggregate.Register(
            householdId,
            "AB-20260228080000",
            "Alice",
            "Bennett",
            occurredUtc);

        var readModel = aggregate.ToReadModel();

        Assert.Equal(householdId, aggregate.HouseholdId);
        Assert.Single(aggregate.Events);
        Assert.Equal("HouseholdRegistered", aggregate.Events[0].EventType);
        Assert.Equal("Alice", readModel.GivenName);
        Assert.Equal("Bennett", readModel.FamilyName);
        Assert.Equal(occurredUtc, readModel.RegisteredAtUtc);
    }

    [Fact]
    public void RehydrateAt_ReturnsNull_WhenNoEventsExistBeforePointInTime()
    {
        var eventHistory = new[]
        {
            new HouseholdEventRecord(
                Guid.NewGuid(),
                "HouseholdRegistered",
                new DateTimeOffset(2026, 2, 28, 10, 0, 0, TimeSpan.Zero),
                "AB-20260228100000",
                "Alice",
                "Bennett"),
        };

        var aggregate = HouseholdAggregate.RehydrateAt(
            eventHistory,
            new DateTimeOffset(2026, 2, 28, 9, 0, 0, TimeSpan.Zero));

        Assert.Null(aggregate);
    }
}
