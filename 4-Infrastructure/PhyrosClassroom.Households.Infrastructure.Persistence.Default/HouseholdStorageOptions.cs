namespace PhyrosClassroom.Households.Infrastructure.Persistence.Default;

public sealed class HouseholdStorageOptions
{
    public const string SectionName = "HouseholdStorage";

    public string BasePath { get; set; } = "App_Data";
}
