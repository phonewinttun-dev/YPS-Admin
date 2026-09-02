# YBS Route Admin Portal

A high-performance transit route and network administration portal built for **Yangon Bus Service (YBS)** public transportation operations. The system delivers centralized management for YBS bus lines, transit stops, route-stop sequences, regional jurisdictions, and integrated Yangon Payment Services (YPS) card retail point-of-sale (POS) networks across the Yangon metropolitan area.

> **Repository Note**: While the solution projects and directory namespaces retain the `YpsAdmin` identifier, this platform is dedicated to **YBS Route Admin** (Yangon Bus Service transit operations, route planning, and fleet card integration).

It features an interactive GIS mapping interface for route-stop assignment powered by Leaflet and OpenStreetMap, bilingual localization (English & Myanmar), real-time operational telemetry, and a custom Swiss Neo-Brutalist design system optimized for long-shift administrative workflows and OS Night Light viewing.

---

## Table of Contents

- [Key Features](#key-features)
- [Design Philosophy](#design-philosophy)
- [Technology Stack](#technology-stack)
- [System Architecture](#system-architecture)
- [Repository Structure](#repository-structure)
- [Database Schema & Data Model](#database-schema--data-model)
- [API Specification](#api-specification)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [1. Database Configuration & Seeding](#1-database-configuration--seeding)
  - [2. Running the API Backend](#2-running-the-api-backend)
  - [3. Compiling Frontend Styles](#3-compiling-frontend-styles)
  - [4. Running the Blazor Client](#4-running-the-blazor-client)
- [Architectural Patterns & Conventions](#architectural-patterns--conventions)
- [Documentation Reference](#documentation-reference)

---

## Key Features

- **Executive Operations Dashboard**: High-level statistical summaries displaying live counts of active YBS bus lines, registered transit stops, retail YPS card outlets, card-accepted fleet metrics, and regional operational distributions.
- **YBS Bus Line Fleet Management**: Comprehensive administration of transit lines with bus numbers, variant indicators, reverse route flags, and YPS card reader acceptance toggles.
- **Bus Stop Directory**: Geolocation management (Latitude/Longitude), dual-language naming (English and Myanmar), road names, and regional township assignments with instant search and pagination.
- **Interactive Route-Stop Mapping**: GIS-based interactive map workspace using Leaflet and OpenStreetMap. Allows administrators to visually assign bus stops to YBS bus lines, inspect transit paths, reorder stop sequences, and remove stops with instant spatial feedback.
- **YPS Retail & Point-of-Sale Network**: Management of card retail counters and physical vendor locations, including geographical coordinates, regional linkage, and association with nearest bus stops and serving YBS bus lines.
- **Regional Zone Administration**: Regional partitioning and jurisdiction management across Yangon transit corridors and townships.
- **Bilingual Localization**: Built-in runtime switching between English (`en`) and Myanmar (`my`), utilizing native Burmese typography (Padauk) and numeral support.
- **Night-Light Friendly Theming**: Custom Swiss Neo-Brutalist UI with Warm Alabaster (Light) and Warm Obsidian (Dark) themes designed specifically to eliminate glare under 2700K–3400K night-light color temperatures.

---

## Technology Stack

### Backend & API

- **Framework**: ASP.NET Core Web API (.NET 10 / C#)
- **Database & ORM**: PostgreSQL with Entity Framework Core 10 (`Npgsql.EntityFrameworkCore.PostgreSQL`)
- **Spatial Support**: `Npgsql.EntityFrameworkCore.PostgreSQL.NetTopologySuite` for PostGIS spatial calculations
- **API Documentation**: Scalar API Reference (`Scalar.AspNetCore`) & Swagger / OpenAPI (`Swashbuckle.AspNetCore`)
- **Caching**: In-memory caching (`IMemoryCache`) for fast operational metric aggregation

### Frontend & Client

- **Framework**: Blazor WebAssembly (.NET 10 / C#)
- **Styling**: Tailwind CSS v4 (`@tailwindcss/cli` v4.3.3) compiled with custom Brutalist design tokens
- **Map & Spatial Interop**: Leaflet 1.9.4 and OpenStreetMap via vanilla JavaScript interop (`route-map.js`)
- **Iconography**: reUI vector SVG stroke icons (`stroke-width="2"`)
- **State & Storage**: Browser `localStorage` abstraction for persisting theme and language selections

### Shared Architecture

- **Contracts**: Standardized `Result`, `Result<T>`, and `PagedResult<T>` response envelopes
- **Data Model**: Database-first scaffolded entity mappings under `YpsAdmin.Database`

---

## System Architecture

The solution uses a decoupled multi-project architecture with feature-based domain organization:

```
┌──────────────────────────────────────────────────────────────────┐
│                   YpsAdmin.Web (Blazor WASM)                     │
│    Pages & Components  │  Tailwind CSS v4  │  Leaflet GIS Map    │
└─────────────────────────────────┬────────────────────────────────┘
                                  │ HTTP / JSON (REST)
┌─────────────────────────────────▼────────────────────────────────┐
│                   YpsAdmin.Api (ASP.NET Core 10)                 │
│    Controllers  │  Scalar & Swagger  │  CORS  │  Middleware      │
└─────────────────────────────────┬────────────────────────────────┘
                                  │ Direct Dependency Injection
┌─────────────────────────────────▼────────────────────────────────┐
│                   YpsAdmin.Domain (Class Library)                │
│    Business Services  │  DTOs  │  Validations  │  FeatureManager │
└──────────────────┬───────────────────────────────┬───────────────┘
                   │                               │
┌──────────────────▼─────────────┐   ┌─────────────▼───────────────┐
│  YpsAdmin.Database (EF Core)   │   │     YpsAdmin.Shared         │
│  AppDbContext │ Tbl* Entities  │   │  Result<T> │ Pagination     │
└──────────────────┬─────────────┘   └─────────────────────────────┘
                   │ Npgsql / PostGIS
┌──────────────────▼─────────────┐
│    PostgreSQL Database Server  │
└────────────────────────────────┘
```

### Request Lifecycle

```
Client Request ──> Controller (Api) ──> Domain Service (Domain) ──> AppDbContext (Database) ──> PostgreSQL
                <── Result<T> / PagedResult<T> (Shared) <───────────────────────────────────────────────┘
```

---

## Repository Structure

```text
YpsAdmin/
├── YpsAdmin.Api/                 # ASP.NET Core 10 Web API project
│   ├── Controllers/             # REST API controllers with dual route support
│   ├── appsettings.json         # Connection strings and runtime logging configuration
│   └── Program.cs               # API pipeline, CORS, Scalar/Swagger endpoints, domain DI
│
├── YpsAdmin.Database/           # Persistence & entity definitions
│   ├── AppDbContextModels/      # Scaffolded DbContext and entity classes (TblBus, TblStore, etc.)
│   └── YpsAdmin.Database.csproj # Npgsql and NetTopologySuite package dependencies
│
├── YpsAdmin.Domain/             # Business logic layer (Feature-based organization)
│   ├── DTOs/                    # Request and response Data Transfer Objects per feature
│   ├── Features/                # Feature-grouped services (Bus, BusRoute, BusStop, Store, Region)
│   │   ├── Bus/                 # Bus line CRUD and query business logic
│   │   ├── BusRoute/            # Route stop mapping and sequencing logic
│   │   ├── BusStop/             # Transit stop query, registration, and region filtering
│   │   ├── Dashboard/           # Aggregated system metrics and statistics
│   │   ├── Region/              # Township and regional zone administration
│   │   ├── Store/               # Retail store and nearest stop/line assignment logic
│   │   └── FeatureManager.cs    # Single entry-point extension method for domain DI
│   └── YpsAdmin.Domain.csproj
│
├── YpsAdmin.Shared/             # Cross-cutting primitives
│   ├── Pagination.cs            # PagedResult<T> and Pagination metadata wrappers
│   ├── PaginationRequest.cs     # Base pagination parameters
│   └── Result.cs                # Unified Result / Result<T> operation envelope
│
├── YpsAdmin.Web/                # Blazor WebAssembly client application
│   ├── Layout/                  # MainLayout, top app bar, breadcrumbs, navigation
│   ├── Pages/                   # Interactive Blazor pages
│   │   ├── BusLines/            # Bus line listing, search, creation, and editing
│   │   ├── BusStops/            # Bus stop directory and location management
│   │   ├── Dashboard/           # Telemetry metrics and overview dashboard
│   │   ├── RouteStops/          # Interactive Leaflet route-stop mapper and reorder tool
│   │   └── YpsStores/           # YPS card retail store directory and stop linking
│   ├── Services/                # Frontend API clients, LanguageService, ThemeService, ToastService
│   ├── Styles/app.css           # Tailwind CSS v4 input file with custom brutalist theme rules
│   ├── wwwroot/                 # Static assets, Leaflet map interop (route-map.js), translations
│   └── package.json             # Tailwind CSS v4 compiler tooling configuration
│
├── DESIGN.md                    # Swiss + Neo-Brutalist design system specifications
├── endpoints.md                 # Complete API endpoints catalog and request/response specifications
├── project_architecture_and_overview.md # Architectural guidelines and feature patterns
├── user_stories.md              # Functional user stories and acceptance criteria
├── ybs_route_db.sql             # PostgreSQL relational schema definition
└── seed.sql                     # Reference data seeding script for Yangon routes and stops
```

---

## Database Schema & Data Model

The PostgreSQL database contains the following core tables:

| Table               | Entity Model        | Description                                                                                                   |
| :------------------ | :------------------ | :------------------------------------------------------------------------------------------------------------ |
| `buses`             | `TblBus`            | YBS transit lines, route bus numbers, variant identifiers, reverse route flags, and YPS card payment support. |
| `bus_stops`         | `TblBusStop`        | Physical transit stops with stop names, geographic coordinates (`lat`, `lon`), and region references.         |
| `bus_routes`        | `TblBusRoute`       | Route-to-stop mapping connecting `bus_id` and `bus_stop_id` ordered by sequence `stop_order`.                 |
| `regions`           | `TblRegion`         | Regional administrative zones and townships within Yangon.                                                    |
| `stores`            | `TblStore`          | Retail YPS card point-of-sale outlets with English/Myanmar names, categories, and coordinates.                |
| `nearest_bus_stops` | `TblNearestBusStop` | Precomputed or assigned nearest bus stops and walking distance (`distance_km`) for each retail store.         |

---

## API Specification

The REST API implements alias routing to support both domain nouns and user story URL conventions. All responses are standardized using the `Result<T>` or `PagedResult<T>` pattern.

Interactive API documentation is accessible in Development mode via **Scalar** at `/scalar/v1` and **Swagger UI** at `/swagger`.

### Key Endpoint Groups

| Module          | Primary Route     | Alias Route              | Notable Operations                                                                                                                                         |
| :-------------- | :---------------- | :----------------------- | :--------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Dashboard**   | `/api/dashboard`  | `/api/dashboard/summary` | `GET` — Aggregated operational statistics and counts                                                                                                       |
| **Bus Lines**   | `/api/buses`      | `/api/bus-lines`         | `GET`, `POST`, `PUT`, `DELETE`, `/search` — YBS bus line CRUD & search                                                                                     |
| **Bus Stops**   | `/api/bus-stops`  | —                        | `GET`, `POST`, `PUT`, `DELETE`, `/search`, `/by-region/{id}`                                                                                               |
| **Route Stops** | `/api/bus-routes` | `/api/route-stops`       | `GET /bus/{id}` — Full ordered route stops<br>`POST /assign` — Batch assign stops<br>`PUT /reorder` — Re-sequence stops<br>`DELETE /bus/{id}/stop/{order}` |
| **YPS Stores**  | `/api/stores`     | `/api/yps-stores`        | `GET`, `POST`, `PUT`, `DELETE`, `/search`<br>`POST /{id}/nearest-stops` — Assign nearby stops<br>`POST /{id}/serving-bus-lines` — Link serving lines       |
| **Regions**     | `/api/regions`    | `/api/townships`         | `GET`, `POST`, `PUT`, `DELETE`, `/search` — Township administration                                                                                        |

> Detailed payload contracts, query parameters, and example responses are documented in [`endpoints.md`](./endpoints.md).

---

## Getting Started

### Prerequisites

Ensure the following tools are installed on your workstation:

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (v10.0.x or higher)
- [PostgreSQL](https://www.postgresql.org/) (v14+ recommended, with PostGIS extension enabled)
- [Node.js](https://nodejs.org/) (v18+ LTS for compiling Tailwind CSS)
- Git

---

### 1. Database Configuration & Seeding

1. Create a new PostgreSQL database (e.g., `ybs_route_db`):

   ```sql
   CREATE DATABASE ybs_route_db;
   ```

2. Initialize schema and seed data:

   ```bash
   psql -U postgres -d ybs_route_db -f ybs_route_db.sql
   psql -U postgres -d ybs_route_db -f seed.sql
   ```

3. Update the connection string in [`YpsAdmin.Api/appsettings.json`](./YpsAdmin.Api/appsettings.json) or user secrets:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=ybs_route_db;Username=postgres;Password=your_password"
     }
   }
   ```

---

### 2. Running the API Backend

Run the backend web API from the repository root:

```bash
dotnet run --project YpsAdmin.Api
```

Once started:

- **API Base URL**: `http://localhost:5214` / `https://localhost:7085`
- **Scalar API Reference**: `http://localhost:5214/scalar/v1`
- **Swagger Documentation**: `http://localhost:5214/swagger`

---

### 3. Compiling Frontend Styles

In a separate terminal, navigate to the `YpsAdmin.Web` directory to restore dependencies and build Tailwind CSS:

```bash
cd YpsAdmin.Web
npm install
npm run build:css
```

During active UI development, run the Tailwind watcher for hot recompilation:

```bash
npm run watch:css
```

---

### 4. Running the Blazor Client

Run the Blazor WebAssembly frontend project:

```bash
dotnet run --project YpsAdmin.Web
```

Open your browser and navigate to the displayed local URL (typically `http://localhost:5000` or `https://localhost:5001`).

---

## Architectural Patterns & Conventions

1. **Feature-Based Domain Organization**: Features are grouped by business capability (`Features/Bus`, `Features/BusRoute`, `Features/Store`) inside `YpsAdmin.Domain`, keeping services, contracts, and DTOs unified rather than scattered across technical layers.
2. **Unified Dependency Injection Entry Point**: All domain services and DbContext configurations are registered via `builder.AddDomain()` in [`YpsAdmin.Domain/Features/FeatureManager.cs`](./YpsAdmin.Domain/Features/FeatureManager.cs).
3. **The Result Pattern**: Business services do not throw exceptions for anticipated validation or workflow failures. Methods return an explicit `Result<T>` or `PagedResult<T>` object containing failure reasons or wrapped data.
4. **DTO Isolation**: Database entities (`Tbl*`) remain private to the persistence and service layers. Controllers exclusively receive request DTOs and return response DTOs.
5. **Direct Service-to-DbContext Flow**: Minimal complexity without heavy CQRS or MediatR overhead: `Controller ──> Service ──> DbContext`.

---

## Documentation Reference

- [`DESIGN.md`](./DESIGN.md) — Swiss + Neo-Brutalist design tokens, typography rules, and component specifications.
- [`endpoints.md`](./endpoints.md) — Comprehensive REST API endpoint catalog, query parameters, and payload schemas.
- [`project_architecture_and_overview.md`](./project_architecture_and_overview.md) — Feature architecture standards and development workflow guidelines.
- [`user_stories.md`](./user_stories.md) — Business requirements, acceptance criteria, and user story traceability.
- [`suggested_features.md`](./suggested_features.md) — Roadmap ideas and future feature proposals.
