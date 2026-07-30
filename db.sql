CREATE EXTENSION IF NOT EXISTS postgis;

CREATE TABLE "TblTownship" (
    "TownshipId" SERIAL PRIMARY KEY,
    "TownshipNameMm" VARCHAR(255) NOT NULL,
    "TownshipNameEn" VARCHAR(255),
    "DeleteFlag" BOOLEAN DEFAULT FALSE
);

CREATE TABLE "TblBusLine" (
    "RouteId" INTEGER PRIMARY KEY,
    "BusNumber" INTEGER NOT NULL,
    "OutboundTitleMm" VARCHAR(255),
    "OutboundTitleEn" VARCHAR(255),
    "ReturnTitleMm" VARCHAR(255),
    "ReturnTitleEn" VARCHAR(255),
    "IsYpsAccepted" BOOLEAN DEFAULT FALSE
);

CREATE TABLE "TblBusStop" (
    "StopId" SERIAL PRIMARY KEY,
    "NameMm" VARCHAR(255) NOT NULL,
    "NameEn" VARCHAR(255),
    "TownshipId" INTEGER REFERENCES "TblTownship"("TownshipId") ON DELETE SET NULL,
    "RoadMm" VARCHAR(255),
    "RoadEn" VARCHAR(255),
    "TotalServingBusLines" INTEGER DEFAULT 0
);

CREATE TABLE "TblRouteStop" (
    "Id" SERIAL PRIMARY KEY,
    "RouteId" INTEGER NOT NULL,
    "StopId" INTEGER,
    "Direction" VARCHAR(20) NOT NULL,
    "StopOrder" INTEGER NOT NULL,
    "StopType" VARCHAR(50),
    CONSTRAINT "FK_RouteStop_Route" FOREIGN KEY ("RouteId") REFERENCES "TblBusLine"("RouteId") ON DELETE CASCADE,
    CONSTRAINT "FK_RouteStop_Stop" FOREIGN KEY ("StopId") REFERENCES "TblBusStop"("StopId") ON DELETE SET NULL,
    CONSTRAINT "UQ_RouteStop_Order" UNIQUE ("RouteId", "Direction", "StopOrder")
);

CREATE TABLE "TblYpsStore" (
    "StoreId" SERIAL PRIMARY KEY,
    "NameMm" VARCHAR(255) NOT NULL,
    "NameEn" VARCHAR(255),
    "Category" VARCHAR(100),
    "TownshipId" INTEGER REFERENCES "TblTownship"("TownshipId") ON DELETE SET NULL,
    "Latitude" NUMERIC(10, 7),
    "Longitude" NUMERIC(10, 7),
    "Geom" GEOMETRY(Point, 4326)
);

CREATE TABLE "TblYpsStore_NearestStop" (
    "Id" SERIAL PRIMARY KEY,
    "StoreId" INTEGER NOT NULL,
    "StopNameMm" VARCHAR(255),
    "StopNameEn" VARCHAR(255),
    "MatchedStopId" INTEGER,
    CONSTRAINT "FK_YpsStore_Nearest_Store" FOREIGN KEY ("StoreId") REFERENCES "TblYpsStore"("StoreId") ON DELETE CASCADE,
    CONSTRAINT "FK_YpsStore_Nearest_Stop" FOREIGN KEY ("MatchedStopId") REFERENCES "TblBusStop"("StopId") ON DELETE SET NULL,
    CONSTRAINT "UQ_YpsStore_Nearest" UNIQUE ("StoreId", "MatchedStopId")
);

CREATE TABLE "TblYpsStore_ServingBusLine" (
    "Id" SERIAL PRIMARY KEY,
    "StoreId" INTEGER NOT NULL,
    "BusNumber" INTEGER NOT NULL,
    "RouteId" INTEGER,
    CONSTRAINT "FK_YpsStore_Serving_Store" FOREIGN KEY ("StoreId") REFERENCES "TblYpsStore"("StoreId") ON DELETE CASCADE,
    CONSTRAINT "FK_YpsStore_Serving_Route" FOREIGN KEY ("RouteId") REFERENCES "TblBusLine"("RouteId") ON DELETE SET NULL,
    CONSTRAINT "UQ_YpsStore_Serving" UNIQUE ("StoreId", "RouteId")
);

CREATE INDEX "idx_route_stops_route_id" ON "TblRouteStop"("RouteId");
CREATE INDEX "idx_route_stops_stop_id" ON "TblRouteStop"("StopId");
CREATE INDEX "idx_yps_nearest_store_id" ON "TblYpsStore_NearestStop"("StoreId");
CREATE INDEX "idx_yps_serving_store_id" ON "TblYpsStore_ServingBusLine"("StoreId");
CREATE INDEX "idx_yps_serving_route_id" ON "TblYpsStore_ServingBusLine"("RouteId");
CREATE INDEX "idx_yps_stores_geom" ON "TblYpsStore" USING GIST ("Geom");
