using PhyrosClassroom.Households.Composition;
using PhyrosClassroom.Households.Presentation.CommandApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHouseholdCommandServices(builder.Configuration);

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    service = "phyrosclassroom-domain-households-command",
    status = "ok",
    timestampUtc = DateTimeOffset.UtcNow,
}));
app.MapHouseholdCommandApi();

app.Run();
