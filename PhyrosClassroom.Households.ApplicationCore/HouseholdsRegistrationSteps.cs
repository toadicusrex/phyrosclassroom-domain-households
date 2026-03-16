using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhyrosClassroom.Households.Composition;
using PhyrosClassroom.Households.Models;
using PhyrosClassroom.Households.Orchestration;
using Reqnroll;

namespace PhyrosClassroom.Households.ApplicationCore;

[Binding]
public sealed class HouseholdsRegistrationSteps
{
    private ServiceProvider? _serviceProvider;
    private HouseholdAggregate? _registeredHousehold;
    private HouseholdReadModel? _queriedHousehold;

    [Given("the Households application composition is configured")]
    public void GivenTheHouseholdsApplicationCompositionIsConfigured()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["HouseholdStorage:BasePath"] = Path.Combine(
                    Path.GetTempPath(),
                    "phyrosclassroom-households-tests",
                    Guid.NewGuid().ToString("N")),
            })
            .Build();

        var services = new ServiceCollection();
        services.AddHouseholdCommandServices(configuration);
        services.AddHouseholdQueryServices(configuration);

        _serviceProvider = services.BuildServiceProvider();
    }

    [When("I register a household named {string} {string}")]
    public async Task WhenIRegisterAHouseholdNamed(string givenName, string familyName)
    {
        var useCase = _serviceProvider!.GetRequiredService<IRegisterHouseholdUseCase>();
        _registeredHousehold = await useCase.ExecuteAsync(new RegisterHouseholdRequest(givenName, familyName));
    }

    [Then("the registered household can be retrieved from the query side")]
    public async Task ThenTheRegisteredHouseholdCanBeRetrievedFromTheQuerySide()
    {
        var useCase = _serviceProvider!.GetRequiredService<IGetHouseholdReadModelByIdUseCase>();
        _queriedHousehold = await useCase.ExecuteAsync(_registeredHousehold!.HouseholdId);

        Assert.NotNull(_queriedHousehold);
        Assert.Equal(_registeredHousehold.HouseholdId, _queriedHousehold!.HouseholdId);
        Assert.Equal(_registeredHousehold.GivenName, _queriedHousehold.GivenName);
        Assert.Equal(_registeredHousehold.FamilyName, _queriedHousehold.FamilyName);
    }
}
