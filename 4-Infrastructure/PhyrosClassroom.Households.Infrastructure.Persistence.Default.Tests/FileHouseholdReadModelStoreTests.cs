using Microsoft.Extensions.Options;
using PhyrosClassroom.Households.Infrastructure.Persistence.Default;
using PhyrosClassroom.Households.Models;

namespace PhyrosClassroom.Households.Infrastructure.Persistence.Default.Tests;

public sealed class FileHouseholdReadModelStoreTests
{
    [Fact]
    public async Task UpsertAsync_PersistsAndReturnsHousehold()
    {
        var basePath = Path.Combine(
            Path.GetTempPath(),
            "phyrosclassroom-households-infrastructure-tests",
            Guid.NewGuid().ToString("N"));

        var store = new FileHouseholdReadModelStore(
            Options.Create(new HouseholdStorageOptions
            {
                BasePath = basePath,
            }));

        var household = new HouseholdReadModel(
            Guid.NewGuid(),
            "AB-20260228083045",
            "Alice",
            "Bennett",
            new DateTimeOffset(2026, 2, 28, 8, 30, 45, TimeSpan.Zero),
            "registrations:default",
            "registration-123:child:0",
            new DateOnly(2012, 4, 16),
            "6",
            "Sarah Bennett",
            "sarah@example.com",
            false,
            null,
            false,
            [],
            []);

        await store.UpsertAsync(household);
        var loadedHousehold = await store.GetByIdAsync(household.HouseholdId);

        Assert.NotNull(loadedHousehold);
        Assert.Equal(household.HouseholdCode, loadedHousehold!.HouseholdCode);
    }
}
