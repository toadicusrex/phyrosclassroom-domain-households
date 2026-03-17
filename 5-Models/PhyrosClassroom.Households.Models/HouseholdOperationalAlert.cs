namespace PhyrosClassroom.Households.Models;

public sealed record HouseholdOperationalAlert(
    Guid HouseholdId,
    string SubjectId,
    string HouseholdName,
    string AlertType,
    string Severity,
    string Message,
    DateTimeOffset UpdatedAtUtc);
