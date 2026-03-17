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
    DateTimeOffset UpdatedAtUtc);
