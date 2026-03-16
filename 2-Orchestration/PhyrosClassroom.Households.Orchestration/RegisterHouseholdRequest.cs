using PhyrosClassroom.Households.Models;

namespace PhyrosClassroom.Households.Orchestration;

public sealed record RegisterHouseholdRequest(
    string GivenName,
    string FamilyName,
    string SubjectId = "",
    string HouseholdName = "",
    bool WantsEmailNotifications = false,
    bool WantsSmsNotifications = false,
    string? OnboardingStatus = null,
    IReadOnlyList<HouseholdContact>? Contacts = null,
    IReadOnlyList<HouseholdStudent>? Students = null,
    Guid? SourceRegistrationId = null,
    string? SourceRegistrationStatus = null,
    DateTimeOffset? SourceRegistrationUpdatedAtUtc = null,
    string? SourceSystem = null,
    string? SourceReference = null);
