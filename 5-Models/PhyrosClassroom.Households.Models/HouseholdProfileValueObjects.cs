namespace PhyrosClassroom.Households.Models;

public sealed record HouseholdContact(
    string FullName,
    string RelationshipToChildren,
    string Email,
    string Phone,
    bool IsPrimaryContact,
    bool WantsPortalAccess,
    string? AlternateEmail = null,
    string? MobilePhone = null,
    string? SecondaryPhone = null,
    string? PreferredContactMethod = null,
    bool IsEmergencyContact = false,
    HouseholdPostalAddress? MailingAddress = null,
    string? Notes = null);

public sealed record HouseholdPostalAddress(
    string AddressLine1,
    string? AddressLine2,
    string City,
    string Region,
    string PostalCode,
    string CountryCode);

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
