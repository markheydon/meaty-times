# MeatyTimes

A simple cooking assistant that calculates roasting instructions for joints of meat based on meat type, weight, and desired doneness.

## Run locally

```powershell
aspire run
# or
dotnet run --project src/MeatyTimes.AppHost
```

Open the **webfrontend** endpoint from the Aspire dashboard.

## Test

```powershell
dotnet test
```

## Feature documentation

- [Roast Calculator quickstart](specs/001-roast-calculator/quickstart.md)
- [Implementation plan](specs/001-roast-calculator/plan.md)

## Stack

- .NET 10 / ASP.NET Core
- Blazor Server + MudBlazor
- .NET Aspire
- xUnit v3
