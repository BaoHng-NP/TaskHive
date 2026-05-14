# TaskHive API

Backend service for the TaskHive freelancer platform. 

## Tech Stack

- **.NET 9** / C#
- **Entity Framework Core 9** (SQL Server, Code-First)
- **SignalR** (Real-time chat)
- **Authentication**: JWT, Google OAuth
- **3rd Party Integrations**: 
  - payOS (Vietnam local payment)
  - Cloudinary (Image/Media storage)
- **Other tools**: AutoMapper, Swagger

## Architecture

The project follows a standard N-Tier architecture to separate concerns:
- `TaskHive.API`: Controllers, Middleware, SignalR Hubs, DI config.
- `TaskHive.Service`: Business logic, third-party service integrations, AutoMapper profiles.
- `TaskHive.Repository`: Data access layer, DbContext, EF Migrations.

## Local Setup

### Requirements
- .NET 9 SDK
- SQL Server

### Steps
1. Clone this repository.
2. Update `appsettings.Development.json` (or configure User Secrets) in the `TaskHive.API` project with your credentials:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TaskHiveDb;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Jwt": {
    "Key": "your_secret_key_minimum_16_characters",
    "Issuer": "taskhive",
    "Audience": "taskhive"
  },
  "Authentication": {
    "Google": {
      "ClientId": "...",
      "ClientSecret": "..."
    }
  },
  "Cloudinary": {
    "CloudName": "...",
    "ApiKey": "...",
    "ApiSecret": "..."
  },
  "PayOs": {
    "ClientId": "...",
    "ApiKey": "...",
    "ChecksumKey": "..."
  }
}
```

3. Run EF Core migrations to set up the database:
```bash
cd TaskHive.API
dotnet ef database update --project ../TaskHive.Repository
```

4. Start the server:
```bash
dotnet run
```
Swagger UI will be available at `https://localhost:<port>/swagger` for API testing.
