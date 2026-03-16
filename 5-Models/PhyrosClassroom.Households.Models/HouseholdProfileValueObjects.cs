namespace PhyrosClassroom.Households.Models;

public sealed record HouseholdContact(
    string FullName,
    string RelationshipToChildren,
    string Email,
    string Phone,
    bool IsPrimaryContact,
    bool WantsPortalAccess);

public sealed record HouseholdStudent(
    Guid? StudentId,
    string StudentCode,
    string GivenName,
    string FamilyName,
    string GradeLevel,
    DateOnly? BirthDate,
    string RelationshipToPrimaryContact);

public sealed record HouseholdDocumentReference(
    string DocumentType,
    string FileName,
    DateTimeOffset UploadedAtUtc,
    string UploadedByUserId);

public sealed record HouseholdRecordNote(
    string Category,
    string Body,
    DateTimeOffset RecordedAtUtc,
    string RecordedByUserId);
