using PhyrosClassroom.Households.Models;

namespace PhyrosClassroom.Households.Orchestration;

public sealed record UpdateHouseholdProfileRequest(
    Guid HouseholdId,
    string HouseholdName,
    bool WantsEmailNotifications,
    bool WantsSmsNotifications,
    string? OnboardingStatus,
    IReadOnlyList<HouseholdContact> Contacts,
    IReadOnlyList<HouseholdStudent> Students,
    Guid? SourceRegistrationId,
    string? SourceRegistrationStatus,
    DateTimeOffset? SourceRegistrationUpdatedAtUtc,
    DateOnly? BirthDate,
    string? GradeLevel,
    string? PrimaryGuardianName,
    string? PrimaryGuardianEmail,
    bool HasMedicalAlert,
    string? MedicalNotes,
    bool HasIep,
    IReadOnlyList<HouseholdDocumentReference> Documents,
    IReadOnlyList<HouseholdRecordNote> Notes);
