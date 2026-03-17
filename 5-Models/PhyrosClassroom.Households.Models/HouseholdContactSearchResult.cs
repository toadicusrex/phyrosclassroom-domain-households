namespace PhyrosClassroom.Households.Models;

public sealed record HouseholdContactSearchResult(
    Guid HouseholdId,
    string SubjectId,
    string HouseholdName,
    string FullName,
    string RelationshipToChildren,
    string Email,
    string Phone,
    bool IsPrimaryContact,
    int StudentCount,
    DateTimeOffset UpdatedAtUtc,
    string? AlternateEmail = null,
    string? MobilePhone = null,
    string? PreferredContactMethod = null,
    bool IsEmergencyContact = false,
    string? City = null,
    string? Region = null);
