namespace PhyrosClassroom.Households.Orchestration;

public sealed record ReviewHouseholdPortalAccessRequest(
    Guid HouseholdId,
    string FullName,
    string? Email,
    string Decision,
    string ReviewedByUserId,
    string? ReviewNotes);
