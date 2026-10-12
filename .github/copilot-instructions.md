# Copilot Instructions for tradeplatform-sus-signupsubscriber-external-function

## Project Overview
This is an isolated-worker Azure Function app (targeting .NET 10) that subscribes to Service Bus messages for REMOS sign-up and establishment create/update events, validates and maps them to Dynamics/CRM DTOs, and forwards results via the Defra CRM client and event store Service Bus sender.

## What This Solution Does
This repository is the Trade Platform SuS (Sign-up Subscriber) integration into IDCOMS (Dynamics CRM). It is independent of SuS and IDCOMS codebases, acting purely as the subscriber/mapper between them:

- The SuS API publishes registration and self-service events (sign-up and establishment create/update) onto Azure Service Bus.
- `RemosSignUpSubscriberServiceBusTriggerFunction` listens on the `defra.trade.sus.remos.signup` queue (sessions disabled) and dispatches each message, by label, to the matching processor:
  - **Sign-up create** — maps the request to an `OrganisationSignup` and inspection locations, then upserts the organisation and creates inspection locations in Dynamics CRM via `ICrmClient`.
  - **Establishment create** — maps the request to an `InspectionLocation` and calls `ICrmClient.CreateAsync`.
  - **Establishment update** — maps the request to an `InspectionLocation`, sets its state/status to inactive, and calls `ICrmClient.UpdateAsync`.
- These processors write to Dynamics CRM but do not forward messages (`ForwardMessage = false`); SuS feedback (e.g. `defra.trade.sus.rejections`, `defra.trade.sus.establishment`) is published by IDCOMS, not this subscriber.
- The shared `Defra.Trade.Common.Functions.Isolated` message executor reports failed messages via Service Bus to the external Events Store application (`defra.trade.events.info`), and this app also sends application logs/telemetry to Application Insights — both are separate from the Dynamics writes and SuS feedback flows.

In short: this function's responsibility is to validate, map, and persist REMOS sign-up/establishment events from SuS into Dynamics CRM (IDCOMS) — it is not responsible for SuS feedback, UI state, or SuS's own SQL storage.

## Tech Stack
- **Runtime**: .NET 10, C# with nullable reference types and implicit usings enabled (`<Nullable>enable</Nullable>`, `<ImplicitUsings>enable</ImplicitUsings>`).
- **Host**: Azure Functions isolated worker model (`Microsoft.Azure.Functions.Worker`, `Microsoft.Azure.Functions.Worker.Sdk`).
- **Messaging**: Azure Service Bus trigger/sender (`Microsoft.Azure.Functions.Worker.Extensions.ServiceBus`, `Azure.Messaging.ServiceBus`), with message execution delegated to `Defra.Trade.Common.Functions.Isolated`.
- **CRM integration**: `Defra.Trade.Crm` client for forwarding mapped requests to Dynamics/CRM.
- **Mapping**: `AutoMapper` (v13) `Profile` classes, configured via `ConfigureMappingExtensions`/`ConfigureMapper()`.
- **Validation**: FluentValidation validators, one per DTO.
- **Health checks**: `Defra.Trade.Common.Function.Health`, registered in `HealthChecksRegistration`.
- **Logging/telemetry**: `Microsoft.ApplicationInsights.WorkerService`, `Microsoft.Azure.Functions.Worker.ApplicationInsights`, `Defra.Trade.Common.Logging`, with structured `[LoggerMessage]`-based logging extensions.
- **Configuration**: `Defra.Trade.Common.AppConfig` (Azure App Configuration + Key Vault) configured in `Program.cs`; local overrides in `local.settings.json` (gitignored).
- **Testing**: xUnit, FakeItEasy, Moq, AutoFixture, Shouldly, `coverlet.collector` for coverage.
- **Build**: Solution file `Defra.Trade.Events.SUS.RemosSignUpSubscriber.sln`, built with Visual Studio or `dotnet build`/`dotnet test`.

## Solution Structure
- `src/Defra.Trade.Events.SUS.RemosSignUpSubscriber` — Azure Functions host (isolated worker). Contains `Program.cs` (host/DI bootstrap), `Functions/` (Service Bus trigger + health check functions), `Infrastructure/` (service registration, AutoMapper configuration, message filters).
- `src/Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application` — Core application logic with no Functions runtime dependency:
  - `Dtos/Inbound` — incoming message contracts (SignUp Create/Update, Establishment Create/Update), grouped by feature folder with `MessageHeader`, `Request`, `TradeParty` per feature.
  - `Dtos/Dynamics` — outbound CRM/Dynamics DTOs and enums.
  - `Mapping` — AutoMapper `Profile` classes, one per request/entity type.
  - `Validators` — FluentValidation validators, one per DTO, plus shared `RemosValidationHelpers`/`RemosValidationMessages`.
  - `Services` — message processors implementing `IMessageProcessor<TRequest, TMessageHeader>` via `BaseMessageProcessor<TRequest, TMessageHeader, TMessageProcessor>`.
  - `Models` — constants and settings (`RemosSignUpSubscriberSettings`, `CrmRequestConstants`, `RemosSignUpServiceHeaderConstants`).
- `test/Defra.Trade.Events.SUS.RemosSignUpSubscriber.Tests` and `test/Defra.Trade.Events.SUS.RemosSignUpSubscriber.Application.Tests` — unit tests mirroring the corresponding `src` project structure.

## Conventions
- All files start with the standard header:
  ```csharp
  // Copyright DEFRA (c). All rights reserved.
  // Licensed under the Open Government License v3.0.
  ```
- File-scoped namespaces are used throughout.
- New inbound request types follow the existing folder-per-feature pattern under `Dtos/Inbound/<Feature>/<Create|Update>/` with `MessageHeader.cs`, `Request.cs`, `TradeParty.cs`.
- New message processors should derive from `BaseMessageProcessor<TRequest, TMessageHeader, TMessageProcessor>` and implement `ProcessAsync`; register dependencies (CRM client, mapper, logger) via constructor and validate with `ArgumentNullException.ThrowIfNull`.
- Mapping is done with AutoMapper `Profile` classes registered via `ConfigureMappingExtensions`/`ConfigureMapper()` in the Functions host project — add a new `Profile` class rather than configuring mappings inline.
- Validation uses FluentValidation; add a new validator class per DTO and reuse `RemosValidationHelpers`/`RemosValidationMessages` for shared rules/messages instead of duplicating logic.
- DI registration is split: `AddServiceRegistrations` (Functions host, `Infrastructure/ServiceRegistrationExtensions.cs`) for infrastructure concerns, `AddApplication` (Application project, `ServiceCollectionExtensions.cs`) for application-layer services — register new services in the appropriate layer.
- Use `[LoggerMessage]`-based extension methods in `Extensions/LoggerExtensions.cs` for structured logging rather than ad-hoc `ILogger` calls where a pattern already exists.
- The Service Bus trigger function (`RemosSignUpSubscriberServiceBusTriggerFunction`) delegates all processing to `IMessageExecutorFactory`/`IMessageExecutor` from `Defra.Trade.Common.Functions.Isolated`; keep the function itself thin (logging + delegation only).

## Build & Test
- Build via the solution file `Defra.Trade.Events.SUS.RemosSignUpSubscriber.sln`.
- Local Functions settings live in `local.settings.json` (gitignored); use `local.settings.example.json` as the template when adding new required configuration values.
- Tests mirror the `src` project structure 1:1 — when adding a class under `src/.../Application/Validators/Foo.cs`, add a corresponding test under `test/.../Validators/FooTests.cs` (same pattern for Mapping, Services, Functions, Infrastructure).
