using System.Text.Json;
using Microsoft.Extensions.Options;
using PhyrosClassroom.Households.Infrastructure.Persistence;
using PhyrosClassroom.Households.Models;

namespace PhyrosClassroom.Households.Infrastructure.Persistence.Default;

public sealed class FileHouseholdReadModelStore(IOptions<HouseholdStorageOptions> options) : IHouseholdReadModelStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
    };

    private static readonly SemaphoreSlim Gate = new(1, 1);

    public async Task UpsertAsync(HouseholdReadModel household, CancellationToken cancellationToken = default)
    {
        var filePath = GetReadModelsPath(options.Value.BasePath);
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

        await Gate.WaitAsync(cancellationToken);
        try
        {
            var households = await ReadInternalAsync(filePath, cancellationToken);
            households[household.HouseholdId] = household;

            await using var stream = File.Create(filePath);
            await JsonSerializer.SerializeAsync(
                stream,
                households.Values.OrderBy(item => item.HouseholdCode).ToList(),
                SerializerOptions,
                cancellationToken);
        }
        finally
        {
            Gate.Release();
        }
    }

    public async Task<HouseholdReadModel?> GetByIdAsync(Guid householdId, CancellationToken cancellationToken = default)
    {
        var filePath = GetReadModelsPath(options.Value.BasePath);

        await Gate.WaitAsync(cancellationToken);
        try
        {
            var households = await ReadInternalAsync(filePath, cancellationToken);
            return households.TryGetValue(householdId, out var household) ? household : null;
        }
        finally
        {
            Gate.Release();
        }
    }

    public async Task<HouseholdReadModel?> GetBySourceReferenceAsync(string sourceSystem, string sourceReference, CancellationToken cancellationToken = default)
    {
        var filePath = GetReadModelsPath(options.Value.BasePath);

        await Gate.WaitAsync(cancellationToken);
        try
        {
            return (await ReadInternalAsync(filePath, cancellationToken))
                .Values
                .FirstOrDefault(item =>
                    string.Equals(item.SourceSystem, sourceSystem, StringComparison.Ordinal) &&
                    string.Equals(item.SourceReference, sourceReference, StringComparison.Ordinal));
        }
        finally
        {
            Gate.Release();
        }
    }

    public async Task<IReadOnlyList<HouseholdReadModel>> ListAsync(CancellationToken cancellationToken = default)
    {
        var filePath = GetReadModelsPath(options.Value.BasePath);

        await Gate.WaitAsync(cancellationToken);
        try
        {
            return (await ReadInternalAsync(filePath, cancellationToken))
                .Values
                .OrderBy(item => item.HouseholdCode)
                .ToList();
        }
        finally
        {
            Gate.Release();
        }
    }

    private static string GetReadModelsPath(string basePath)
    {
        return Path.Combine(basePath, "households", "read-models.json");
    }

    private static async Task<Dictionary<Guid, HouseholdReadModel>> ReadInternalAsync(string filePath, CancellationToken cancellationToken)
    {
        if (!File.Exists(filePath))
        {
            return [];
        }

        await using var stream = File.OpenRead(filePath);
        var households = await JsonSerializer.DeserializeAsync<List<HouseholdReadModel>>(stream, SerializerOptions, cancellationToken);
        return (households ?? []).ToDictionary(item => item.HouseholdId);
    }
}
