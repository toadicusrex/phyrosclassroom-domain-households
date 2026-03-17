namespace PhyrosClassroom.Households.Models;

public sealed record HouseholdPortalAccessReviewItem(
    Guid HouseholdId,
    string SubjectId,
    string HouseholdName,
    string FullName,
    string RelationshipToChildren,
    string? Email,
    string? AlternateEmail,
    string? MobilePhone,
    bool IsPrimaryContact,
    bool IsEmergencyContact,
    bool ReadyForInvite,
    string InviteReadiness,
    string PortalAccessStatus,
    string? PortalAccessReviewNotes,
    DateTimeOffset UpdatedAtUtc);
