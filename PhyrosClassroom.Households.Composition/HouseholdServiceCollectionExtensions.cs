using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhyrosClassroom.Households.Engines;
using PhyrosClassroom.Households.Engines.Default;
using PhyrosClassroom.Households.Infrastructure.Persistence;
using PhyrosClassroom.Households.Infrastructure.Persistence.Default;
using PhyrosClassroom.Households.Orchestration;
using PhyrosClassroom.Households.Orchestration.Default;

namespace PhyrosClassroom.Households.Composition;

public static class HouseholdServiceCollectionExtensions
{
    public static IServiceCollection AddHouseholdCommandServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHouseholdSharedServices(configuration);
        services.AddScoped<IRegisterHouseholdUseCase, RegisterHouseholdUseCase>();
        services.AddScoped<IUpdateHouseholdProfileUseCase, UpdateHouseholdProfileUseCase>();
        services.AddScoped<IGetHouseholdByIdUseCase, GetHouseholdByIdUseCase>();
        services.AddScoped<IGetHouseholdAtPointInTimeUseCase, GetHouseholdAtPointInTimeUseCase>();
        services.AddScoped<IGetHouseholdEventHistoryUseCase, GetHouseholdEventHistoryUseCase>();

        return services;
    }

    public static IServiceCollection AddHouseholdQueryServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHouseholdSharedServices(configuration);
        services.AddScoped<IListHouseholdsUseCase, ListHouseholdsUseCase>();
        services.AddScoped<ISearchHouseholdContactsUseCase, SearchHouseholdContactsUseCase>();
        services.AddScoped<IListHouseholdOperationalAlertsUseCase, ListHouseholdOperationalAlertsUseCase>();
        services.AddScoped<IGetHouseholdReadModelByIdUseCase, GetHouseholdReadModelByIdUseCase>();
        services.AddScoped<IGetHouseholdReadModelBySubjectIdUseCase, GetHouseholdReadModelBySubjectIdUseCase>();

        return services;
    }

    private static IServiceCollection AddHouseholdSharedServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<HouseholdStorageOptions>()
            .Bind(configuration.GetSection(HouseholdStorageOptions.SectionName));

        services.AddSingleton<IHouseholdEventStore, FileHouseholdEventStore>();
        services.AddSingleton<IHouseholdReadModelStore, FileHouseholdReadModelStore>();
        services.AddSingleton<IHouseholdHydratedModelCache, InMemoryHouseholdHydratedModelCache>();
        services.AddSingleton<IHouseholdCodeGenerator, HouseholdCodeGenerator>();

        return services;
    }
}
