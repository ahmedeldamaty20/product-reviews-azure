# Product Reviews Platform

A cloud-native product reviews application built with **ASP.NET Core MVC** and **Microsoft Azure**.
Users sign in, submit product reviews with images, and the images are processed asynchronously in the background.

> 🚧 Work in progress. This README is updated as each phase is completed.

## Architecture

_(Architecture diagram will be added in Phase 9)_

## Tech Stack

| Area | Technology |
|---|---|
| Web App | ASP.NET Core MVC, Entity Framework Core |
| Hosting | Azure App Service, Azure Container Apps |
| Database | Azure SQL Database (Serverless) |
| Storage | Azure Blob Storage, Azure Front Door |
| Async Processing | Azure Storage Queue, Azure Functions, Table Storage |
| Identity | Microsoft Entra External ID |
| Security | Managed Identity, Azure RBAC, Key Vault |
| CI/CD | GitHub Actions |
| Observability | Application Insights, Log Analytics, KQL |
| IaC | Bicep |

## Progress

- [x] Phase 0: Setup
- [ ] Phase 1: Core app + Azure SQL
- [ ] Phase 2: Blob Storage + Front Door
- [ ] Phase 3: Security & Identity
- [ ] Phase 4: Event-driven processing
- [ ] Phase 5: CI/CD
- [ ] Phase 6: Containers
- [ ] Phase 7: Observability & KQL
- [ ] Phase 8: Infrastructure as Code
- [ ] Phase 9: Documentation

## Naming Convention

`{resource-type}-{app-name}-{region}-{environment}-{instance}`

Example: `rg-reviews-eastus-dev-001`

## Getting Started

_(Will be added in Phase 9)_

## Challenges & Lessons Learned

_(Add a new entry every time you hit a real problem and solve it.)_

## License

MIT