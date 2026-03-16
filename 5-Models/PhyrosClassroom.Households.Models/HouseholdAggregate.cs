namespace PhyrosClassroom.Households.Models;

public sealed class HouseholdAggregate
{
    private readonly List<HouseholdEventRecord> _events = [];

    private HouseholdAggregate()
    {
    }

    public Guid HouseholdId { get; private set; }
    public string HouseholdCode { get; private set; } = string.Empty;
    public string SubjectId { get; private set; } = string.Empty;
    public Guid? SourceRegistrationId { get; private set; }
    public string? SourceRegistrationStatus { get; private set; }
    public DateTimeOffset? SourceRegistrationUpdatedAtUtc { get; private set; }
    public string HouseholdName { get; private set; } = string.Empty;
    public bool WantsEmailNotifications { get; private set; }
    public bool WantsSmsNotifications { get; private set; }
    public string? OnboardingStatus { get; private set; }
    public IReadOnlyList<HouseholdContact> Contacts { get; private set; } = [];
    public IReadOnlyList<HouseholdStudent> Students { get; private set; } = [];
    public string GivenName { get; private set; } = string.Empty;
    public string FamilyName { get; private set; } = string.Empty;
    public string? SourceSystem { get; private set; }
    public string? SourceReference { get; private set; }
    public DateOnly? BirthDate { get; private set; }
    public string? GradeLevel { get; private set; }
    public string? PrimaryGuardianName { get; private set; }
    public string? PrimaryGuardianEmail { get; private set; }
    public bool HasMedicalAlert { get; private set; }
    public string? MedicalNotes { get; private set; }
    public bool HasIep { get; private set; }
    public IReadOnlyList<HouseholdDocumentReference> Documents { get; private set; } = [];
    public IReadOnlyList<HouseholdRecordNote> Notes { get; private set; } = [];
    public DateTimeOffset RegisteredAtUtc { get; private set; }
    public IReadOnlyList<HouseholdEventRecord> Events => _events;

    public static HouseholdAggregate Register(
        Guid householdId,
        string householdCode,
        string givenName,
        string familyName,
        DateTimeOffset occurredUtc,
        string subjectId = "",
        string householdName = "",
        bool wantsEmailNotifications = false,
        bool wantsSmsNotifications = false,
        string? onboardingStatus = null,
        IReadOnlyList<HouseholdContact>? contacts = null,
        IReadOnlyList<HouseholdStudent>? students = null,
        Guid? sourceRegistrationId = null,
        string? sourceRegistrationStatus = null,
        DateTimeOffset? sourceRegistrationUpdatedAtUtc = null,
        string? sourceSystem = null,
        string? sourceReference = null)
    {
        var aggregate = new HouseholdAggregate();
        aggregate.Apply(
            new HouseholdEventRecord(
                householdId,
                "HouseholdRegistered",
                occurredUtc,
                householdCode,
                givenName.Trim(),
                familyName.Trim(),
                string.IsNullOrWhiteSpace(sourceSystem) ? null : sourceSystem.Trim(),
                string.IsNullOrWhiteSpace(sourceReference) ? null : sourceReference.Trim(),
                null,
                null,
                null,
                null,
                false,
                null,
                false,
                null,
                null,
                string.IsNullOrWhiteSpace(subjectId) ? string.Empty : subjectId.Trim(),
                sourceRegistrationId,
                string.IsNullOrWhiteSpace(sourceRegistrationStatus) ? null : sourceRegistrationStatus.Trim(),
                sourceRegistrationUpdatedAtUtc,
                string.IsNullOrWhiteSpace(householdName) ? string.Empty : householdName.Trim(),
                wantsEmailNotifications,
                wantsSmsNotifications,
                string.IsNullOrWhiteSpace(onboardingStatus) ? null : onboardingStatus.Trim(),
                contacts?.ToArray(),
                students?.ToArray()));

        return aggregate;
    }

    public static HouseholdAggregate? Rehydrate(IEnumerable<HouseholdEventRecord> eventHistory)
    {
        var aggregate = new HouseholdAggregate();

        foreach (var householdEvent in eventHistory.OrderBy(eventItem => eventItem.OccurredUtc))
        {
            aggregate.Apply(householdEvent);
        }

        return aggregate._events.Count == 0 ? null : aggregate;
    }

    public static HouseholdAggregate? RehydrateAt(
        IEnumerable<HouseholdEventRecord> eventHistory,
        DateTimeOffset pointInTimeUtc)
    {
        return Rehydrate(eventHistory.Where(eventItem => eventItem.OccurredUtc <= pointInTimeUtc));
    }

    public HouseholdReadModel ToReadModel()
    {
        return new HouseholdReadModel(
            HouseholdId,
            HouseholdCode,
            GivenName,
            FamilyName,
            RegisteredAtUtc,
            SourceSystem,
            SourceReference,
            BirthDate,
            GradeLevel,
            PrimaryGuardianName,
            PrimaryGuardianEmail,
            HasMedicalAlert,
            MedicalNotes,
            HasIep,
            Documents,
            Notes,
            SubjectId,
            SourceRegistrationId,
            SourceRegistrationStatus,
            SourceRegistrationUpdatedAtUtc,
            HouseholdName,
            WantsEmailNotifications,
            WantsSmsNotifications,
            OnboardingStatus,
            Contacts,
            Students,
            _events.LastOrDefault()?.OccurredUtc ?? RegisteredAtUtc);
    }

    public void UpdateProfile(
        string householdName,
        bool wantsEmailNotifications,
        bool wantsSmsNotifications,
        string? onboardingStatus,
        IReadOnlyList<HouseholdContact> contacts,
        IReadOnlyList<HouseholdStudent> students,
        Guid? sourceRegistrationId,
        string? sourceRegistrationStatus,
        DateTimeOffset? sourceRegistrationUpdatedAtUtc,
        DateOnly? birthDate,
        string? gradeLevel,
        string? primaryGuardianName,
        string? primaryGuardianEmail,
        bool hasMedicalAlert,
        string? medicalNotes,
        bool hasIep,
        IReadOnlyList<HouseholdDocumentReference> documents,
        IReadOnlyList<HouseholdRecordNote> notes,
        DateTimeOffset occurredUtc)
    {
        Apply(new HouseholdEventRecord(
            HouseholdId,
            "HouseholdProfileUpdated",
            occurredUtc,
            HouseholdCode,
            GivenName,
            FamilyName,
            SourceSystem,
            SourceReference,
            birthDate,
            string.IsNullOrWhiteSpace(gradeLevel) ? null : gradeLevel.Trim(),
            string.IsNullOrWhiteSpace(primaryGuardianName) ? null : primaryGuardianName.Trim(),
            string.IsNullOrWhiteSpace(primaryGuardianEmail) ? null : primaryGuardianEmail.Trim(),
            hasMedicalAlert,
            string.IsNullOrWhiteSpace(medicalNotes) ? null : medicalNotes.Trim(),
            hasIep,
            documents.ToArray(),
            notes.ToArray(),
            SubjectId,
            sourceRegistrationId,
            string.IsNullOrWhiteSpace(sourceRegistrationStatus) ? null : sourceRegistrationStatus.Trim(),
            sourceRegistrationUpdatedAtUtc,
            string.IsNullOrWhiteSpace(householdName) ? HouseholdName : householdName.Trim(),
            wantsEmailNotifications,
            wantsSmsNotifications,
            string.IsNullOrWhiteSpace(onboardingStatus) ? null : onboardingStatus.Trim(),
            contacts.ToArray(),
            students.ToArray()));
    }

    private void Apply(HouseholdEventRecord householdEvent)
    {
        HouseholdId = householdEvent.HouseholdId;
        HouseholdCode = householdEvent.HouseholdCode;
        SubjectId = householdEvent.SubjectId;
        SourceRegistrationId = householdEvent.SourceRegistrationId;
        SourceRegistrationStatus = householdEvent.SourceRegistrationStatus;
        SourceRegistrationUpdatedAtUtc = householdEvent.SourceRegistrationUpdatedAtUtc;
        HouseholdName = householdEvent.HouseholdName;
        WantsEmailNotifications = householdEvent.WantsEmailNotifications;
        WantsSmsNotifications = householdEvent.WantsSmsNotifications;
        OnboardingStatus = householdEvent.OnboardingStatus;
        Contacts = householdEvent.Contacts ?? [];
        Students = householdEvent.Students ?? [];
        GivenName = householdEvent.GivenName;
        FamilyName = householdEvent.FamilyName;
        SourceSystem = householdEvent.SourceSystem;
        SourceReference = householdEvent.SourceReference;
        BirthDate = householdEvent.BirthDate;
        GradeLevel = householdEvent.GradeLevel;
        PrimaryGuardianName = householdEvent.PrimaryGuardianName;
        PrimaryGuardianEmail = householdEvent.PrimaryGuardianEmail;
        HasMedicalAlert = householdEvent.HasMedicalAlert;
        MedicalNotes = householdEvent.MedicalNotes;
        HasIep = householdEvent.HasIep;
        Documents = householdEvent.Documents ?? [];
        Notes = householdEvent.Notes ?? [];

        if (RegisteredAtUtc == default)
        {
            RegisteredAtUtc = householdEvent.OccurredUtc;
        }

        _events.Add(householdEvent);
    }
}
