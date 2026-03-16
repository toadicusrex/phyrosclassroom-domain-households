using PhyrosClassroom.Households.Composition;
using PhyrosClassroom.Households.Presentation.QueryApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHouseholdQueryServices(builder.Configuration);

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    service = "phyrosclassroom-domain-households-query",
    status = "ok",
    timestampUtc = DateTimeOffset.UtcNow,
}));
app.MapHouseholdQueryApi();

app.Run();
