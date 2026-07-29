# 📡 YpsAdmin REST API Endpoints Specification

This document lists all REST API endpoints implemented for the YpsAdmin system, grouped by feature modules.

---

## 1. 🚌 Bus Line Management (`/api/bus-lines`)

| Method | Endpoint | Description | Story ID | Request Body / Query | Response |
| :--- | :--- | :--- | :--- | :--- | :--- |
| `GET` | `/api/bus-lines` | Get paginated list of bus lines with optional search | **US-01** | Query: `pageNumber`, `pageSize`, `searchBusNumber` | `Result<PagedList<BusLineDto>>` |
| `GET` | `/api/bus-lines/{id}` | Get bus line details by ID | **US-01** | Path: `id` | `Result<BusLineDto>` |
| `POST` | `/api/bus-lines` | Create a new bus line | **US-02** | Body: `CreateBusLineRequest` | `Result<BusLineDto>` |
| `PUT` | `/api/bus-lines/{id}` | Update an existing bus line | **US-03** | Path: `id`, Body: `UpdateBusLineRequest` | `Result<BusLineDto>` |
| `DELETE` | `/api/bus-lines/{id}` | Delete a bus line | **US-04** | Path: `id` | `Result<bool>` |

---

## 2. 🚏 Bus Stop Management (`/api/bus-stops`)

| Method | Endpoint | Description | Story ID | Request Body / Query | Response |
| :--- | :--- | :--- | :--- | :--- | :--- |
| `GET` | `/api/bus-stops` | Get paginated list of bus stops with optional search | **US-05** | Query: `pageNumber`, `pageSize`, `searchStopName` | `Result<PagedList<BusStopDto>>` |
| `GET` | `/api/bus-stops/{id}` | Get bus stop details by ID | **US-05** | Path: `id` | `Result<BusStopDto>` |
| `POST` | `/api/bus-stops` | Create a new bus stop (Validates unique Stop ID) | **US-06** | Body: `CreateBusStopRequest` | `Result<BusStopDto>` |
| `PUT` | `/api/bus-stops/{id}` | Update an existing bus stop | **US-07** | Path: `id`, Body: `UpdateBusStopRequest` | `Result<BusStopDto>` |
| `DELETE` | `/api/bus-stops/{id}` | Delete a bus stop | **US-07** | Path: `id` | `Result<bool>` |

---

## 3. 🗺️ Route-Stop Mapping (`/api/route-stops`)

| Method | Endpoint | Description | Story ID | Request Body / Query | Response |
| :--- | :--- | :--- | :--- | :--- | :--- |
| `GET` | `/api/route-stops/bus-line/{busLineId}` | View full route (Outbound and Return stops sorted by `stop_order`) | **US-10** | Path: `busLineId` | `Result<FullRouteResponseDto>` |
| `POST` | `/api/route-stops/assign` | Assign bus stops to a bus line with direction and order | **US-08** | Body: `AssignRouteStopsRequest` | `Result<bool>` |
| `PUT` | `/api/route-stops/reorder` | Update the sequence order of stops within a route | **US-09** | Body: `ReorderRouteStopsRequest` | `Result<bool>` |
| `DELETE` | `/api/route-stops/{routeStopId}` | Remove a bus stop from a route mapping | **US-08** | Path: `routeStopId` | `Result<bool>` |

---

## 4. 🏪 YPS Store Management (`/api/yps-stores`)

| Method | Endpoint | Description | Story ID | Request Body / Query | Response |
| :--- | :--- | :--- | :--- | :--- | :--- |
| `GET` | `/api/yps-stores` | Get paginated list of YPS stores | **US-11** | Query: `pageNumber`, `pageSize`, `searchName` | `Result<PagedList<YpsStoreDto>>` |
| `GET` | `/api/yps-stores/{id}` | Get YPS store details by ID (includes coordinates & linked stops/lines) | **US-11** | Path: `id` | `Result<YpsStoreDto>` |
| `POST` | `/api/yps-stores` | Create a new YPS store (converts Lat/Lon to PostGIS Point) | **US-12** | Body: `CreateYpsStoreRequest` | `Result<YpsStoreDto>` |
| `PUT` | `/api/yps-stores/{id}` | Update YPS store details & coordinates | **US-12** | Path: `id`, Body: `UpdateYpsStoreRequest` | `Result<YpsStoreDto>` |
| `DELETE` | `/api/yps-stores/{id}` | Delete a YPS store | **US-12** | Path: `id` | `Result<bool>` |
| `POST` | `/api/yps-stores/{id}/nearest-stops` | Assign nearest bus stops to a YPS store | **US-13** | Path: `id`, Body: `AssignNearestStopsRequest` | `Result<bool>` |
| `POST` | `/api/yps-stores/{id}/serving-bus-lines` | Assign serving bus lines to a YPS store | **US-14** | Path: `id`, Body: `AssignServingBusLinesRequest` | `Result<bool>` |
