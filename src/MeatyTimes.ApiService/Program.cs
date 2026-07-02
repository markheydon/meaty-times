using MeatyTimes.ApiService.Endpoints;
using MeatyTimes.Core;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddMeatyTimesCore();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () => "MeatyTimes API is running.");

app.MapRoastEndpoints();
app.MapDefaultEndpoints();

app.Run();
