using System.Text.Json;
using Microsoft.Extensions.Options;
using PhyrosClassroom.Households.Infrastructure.Persistence;
using PhyrosClassroom.Households.Models;

namespace PhyrosClassroom.Households.Infrastructure.Persistence.Default;

public sealed class FileHouseholdEventStore(IOptions<HouseholdStorageOptions> options) : IHouseholdEventStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
    };

    private static readonly SemaphoreSlim Gate = new(1, 1);

    public async Task AppendAsync(
        Guid householdId,
        IReadOnlyList<HouseholdEventRecord> events,
        CancellationToken cancellationToken = default)
    {
        var filePath = GetEventsPath(options.Value.BasePath, householdId);
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

        await Gate.WaitAsync(cancellationToken);
        try
        {
            var existingEvents = await ReadInternalAsync(filePath, cancellationToken);
            existingEvents.AddRange(events);

            await using var stream = File.Create(filePath);
            await JsonSerializer.SerializeAsync(stream, existingEvents, SerializerOptions, cancellationToken);
        }
        finally
        {
            Gate.Release();
        }
    }

    public async Task<IReadOnlyList<HouseholdEventRecord>> GetByIdAsync(Guid householdId, CancellationToken cancellationToken = default)
    {
        var filePath = GetEventsPath(options.Value.BasePath, householdId);

        await Gate.WaitAsync(cancellationToken);
        try
        {
            return await ReadInternalAsync(filePath, cancellationToken);
        }
        finally
        {
            Gate.Release();
        }
    }

    private static string GetEventsPath(string basePath, Guid householdId)
    {
        return Path.Combine(basePath, "households", "events", $"{householdId:N}.json");
    }

    private static async Task<List<HouseholdEventRecord>> ReadInternalAsync(string filePath, CancellationToken cancellationToken)
    {
        if (!File.Exists(filePath))
        {
            return [];
        }

        await using var stream = File.OpenRead(filePath);
        var events = await JsonSerializer.DeserializeAsync<List<HouseholdEventRecord>>(stream, SerializerOptions, cancellationToken);
        return events ?? [];
    }
}
