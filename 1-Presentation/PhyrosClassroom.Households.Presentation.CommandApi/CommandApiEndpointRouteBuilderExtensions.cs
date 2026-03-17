using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PhyrosClassroom.Households.Orchestration;

namespace PhyrosClassroom.Households.Presentation.CommandApi;

public static class CommandApiEndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapHouseholdCommandApi(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/command/households");

        group.MapPost("/", async (
            RegisterHouseholdRequest request,
            IRegisterHouseholdUseCase useCase,
            CancellationToken cancellationToken) =>
        {
            var household = await useCase.ExecuteAsync(request, cancellationToken);
            return Results.Created($"/command/households/{household.HouseholdId}", household);
        });

        group.MapGet("/{householdId:guid}", async (
            Guid householdId,
            IGetHouseholdByIdUseCase useCase,
            CancellationToken cancellationToken) =>
        {
            var household = await useCase.ExecuteAsync(householdId, cancellationToken);
            return household is null ? Results.NotFound() : Results.Ok(household);
        });

        group.MapGet("/{householdId:guid}/point-in-time", async (
            Guid householdId,
            DateTimeOffset at,
            IGetHouseholdAtPointInTimeUseCase useCase,
            CancellationToken cancellationToken) =>
        {
            var household = await useCase.ExecuteAsync(householdId, at, cancellationToken);
            return household is null ? Results.NotFound() : Results.Ok(household);
        });

        group.MapGet("/{householdId:guid}/history", async (
            Guid householdId,
            IGetHouseholdEventHistoryUseCase useCase,
            CancellationToken cancellationToken) =>
        {
            var history = await useCase.ExecuteAsync(householdId, cancellationToken);
            return Results.Ok(history);
        });

        group.MapPut("/{householdId:guid}/profile", async (
            Guid householdId,
            UpdateHouseholdProfileInput input,
            IUpdateHouseholdProfileUseCase useCase,
            CancellationToken cancellationToken) =>
        {
            var household = await useCase.ExecuteAsync(
                new UpdateHouseholdProfileRequest(
                    householdId,
                    input.HouseholdName,
                    input.WantsEmailNotifications,
                    input.WantsSmsNotifications,
                    input.OnboardingStatus,
                    input.Contacts.Select(contact => new Households.Models.HouseholdContact(
                        contact.FullName,
                        contact.RelationshipToChildren,
                        contact.Email,
                        contact.Phone,
                        contact.IsPrimaryContact,
                        contact.WantsPortalAccess,
                        contact.AlternateEmail,
                        contact.MobilePhone,
                        contact.SecondaryPhone,
                        contact.PreferredContactMethod,
                        contact.IsEmergencyContact,
                        contact.MailingAddress is null ? null : new Households.Models.HouseholdPostalAddress(
                            contact.MailingAddress.AddressLine1,
                            contact.MailingAddress.AddressLine2,
                            contact.MailingAddress.City,
                            contact.MailingAddress.Region,
                            contact.MailingAddress.PostalCode,
                            contact.MailingAddress.CountryCode),
                        contact.Notes,
                        contact.PortalAccessStatus,
                        contact.PortalAccessReviewedAtUtc,
                        contact.PortalAccessReviewedByUserId,
                        contact.PortalAccessReviewNotes)).ToArray(),
                    input.Students.Select(student => new Households.Models.HouseholdStudent(
                        student.StudentId,
                        student.StudentCode,
                        student.GivenName,
                        student.FamilyName,
                        student.GradeLevel,
                        student.BirthDate,
                        student.RelationshipToPrimaryContact)).ToArray(),
                    input.SourceRegistrationId,
                    input.SourceRegistrationStatus,
                    input.SourceRegistrationUpdatedAtUtc,
                    input.BirthDate,
                    input.GradeLevel,
                    input.PrimaryGuardianName,
                    input.PrimaryGuardianEmail,
                    input.HasMedicalAlert,
                    input.MedicalNotes,
                    input.HasIep,
                    input.Documents.Select(document => new Households.Models.HouseholdDocumentReference(
                        document.DocumentType,
                        document.FileName,
                        document.UploadedAtUtc,
                        document.UploadedByUserId)).ToArray(),
                    input.Notes.Select(note => new Households.Models.HouseholdRecordNote(
                        note.Category,
                        note.Body,
                        note.RecordedAtUtc,
                        note.RecordedByUserId)).ToArray()),
                cancellationToken);

            return Results.Ok(household);
        });

        group.MapPost("/{householdId:guid}/portal-access-review", async (
            Guid householdId,
            ReviewHouseholdPortalAccessInput input,
            IReviewHouseholdPortalAccessUseCase useCase,
            CancellationToken cancellationToken) =>
        {
            var household = await useCase.ExecuteAsync(
                new ReviewHouseholdPortalAccessRequest(
                    householdId,
                    input.FullName,
                    input.Email,
                    input.Decision,
                    input.ReviewedByUserId,
                    input.ReviewNotes),
                cancellationToken);

            return Results.Ok(household);
        });

        return endpoints;
    }
}

public sealed class UpdateHouseholdProfileInput
{
    public string HouseholdName { get; set; } = string.Empty;
    public bool WantsEmailNotifications { get; set; }
    public bool WantsSmsNotifications { get; set; }
    public string? OnboardingStatus { get; set; }
    public Guid? SourceRegistrationId { get; set; }
    public string? SourceRegistrationStatus { get; set; }
    public DateTimeOffset? SourceRegistrationUpdatedAtUtc { get; set; }
    public List<HouseholdContactInput> Contacts { get; set; } = [];
    public List<HouseholdStudentInput> Students { get; set; } = [];
    public DateOnly? BirthDate { get; set; }
    public string? GradeLevel { get; set; }
    public string? PrimaryGuardianName { get; set; }
    public string? PrimaryGuardianEmail { get; set; }
    public bool HasMedicalAlert { get; set; }
    public string? MedicalNotes { get; set; }
    public bool HasIep { get; set; }
    public List<HouseholdDocumentInput> Documents { get; set; } = [];
    public List<HouseholdNoteInput> Notes { get; set; } = [];
}

public sealed class HouseholdContactInput
{
    public string FullName { get; set; } = string.Empty;
    public string RelationshipToChildren { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public bool IsPrimaryContact { get; set; }
    public bool WantsPortalAccess { get; set; }
    public string? AlternateEmail { get; set; }
    public string? MobilePhone { get; set; }
    public string? SecondaryPhone { get; set; }
    public string? PreferredContactMethod { get; set; }
    public bool IsEmergencyContact { get; set; }
    public HouseholdMailingAddressInput? MailingAddress { get; set; }
    public string? Notes { get; set; }
    public string? PortalAccessStatus { get; set; }
    public DateTimeOffset? PortalAccessReviewedAtUtc { get; set; }
    public string? PortalAccessReviewedByUserId { get; set; }
    public string? PortalAccessReviewNotes { get; set; }
}

public sealed class HouseholdMailingAddressInput
{
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string CountryCode { get; set; } = "US";
}

public sealed class HouseholdStudentInput
{
    public Guid? StudentId { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string GivenName { get; set; } = string.Empty;
    public string FamilyName { get; set; } = string.Empty;
    public string GradeLevel { get; set; } = string.Empty;
    public DateOnly? BirthDate { get; set; }
    public string RelationshipToPrimaryContact { get; set; } = string.Empty;
}

public sealed class HouseholdDocumentInput
{
    public string DocumentType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public DateTimeOffset UploadedAtUtc { get; set; }
    public string UploadedByUserId { get; set; } = string.Empty;
}

public sealed class HouseholdNoteInput
{
    public string Category { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTimeOffset RecordedAtUtc { get; set; }
    public string RecordedByUserId { get; set; } = string.Empty;
}

public sealed class ReviewHouseholdPortalAccessInput
{
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Decision { get; set; } = "Approved";
    public string ReviewedByUserId { get; set; } = string.Empty;
    public string? ReviewNotes { get; set; }
}
