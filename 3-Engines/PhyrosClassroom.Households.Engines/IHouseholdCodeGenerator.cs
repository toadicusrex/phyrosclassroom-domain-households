namespace PhyrosClassroom.Households.Engines;

public interface IHouseholdCodeGenerator
{
    string GenerateCode(string givenName, string familyName, DateTimeOffset occurredUtc);
}
