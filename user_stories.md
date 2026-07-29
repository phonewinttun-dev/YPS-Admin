# 📖 User Stories

## 1. Feature: Bus Line Management

- **Story ID: US-01**
  - **Role (As a):** System Admin
  - **Action (I want to):** view a list of all bus lines
  - **Benefit (So that):** I can see all available routes in the system.
  - **Acceptance Criteria (AC):**
    - Display a list/grid of `TblBusLine` records.
    - Include `Route ID`, `Bus Number`, and `YPS Accepted` status.
    - Support pagination and search by bus number.

- **Story ID: US-02**
  - **Role (As a):** System Admin
  - **Action (I want to):** add a new bus line
  - **Benefit (So that):** I can keep the transport network up to date.
  - **Acceptance Criteria (AC):**
    - Provide a form to input `Route ID`, `Bus Number`, `Outbound Titles`, and `Return Titles`.
    - Include a toggle for `YPS Accepted`.
    - Save data to the database successfully.

- **Story ID: US-03**
  - **Role (As a):** System Admin
  - **Action (I want to):** update an existing bus line
  - **Benefit (So that):** I can correct mistakes or update YPS acceptance status.
  - **Acceptance Criteria (AC):**
    - Clicking "Edit" opens a form populated with existing data.
    - Changes are saved correctly without duplicating records.

- **Story ID: US-04**
  - **Role (As a):** System Admin
  - **Action (I want to):** delete a retired bus line
  - **Benefit (So that):** I can remove obsolete routes from the system.
  - **Acceptance Criteria (AC):**
    - Clicking "Delete" prompts a confirmation dialog.
    - On confirm, the record is removed from `TblBusLine`.

---

## 2. Feature: Bus Stop Management

- **Story ID: US-05**
  - **Role (As a):** System Admin
  - **Action (I want to):** view a list of all bus stops
  - **Benefit (So that):** I can browse and search through all registered stops.
  - **Acceptance Criteria (AC):**
    - Display a list of `TblBusStop` records.
    - Show `Stop Name`, `Township`, and `Road`.
    - Support search by stop name.

- **Story ID: US-06**
  - **Role (As a):** System Admin
  - **Action (I want to):** add a new bus stop
  - **Benefit (So that):** I can register new stops added by the government.
  - **Acceptance Criteria (AC):**
    - Form includes fields for `Stop ID`, `Name (MM/EN)`, `Township (MM/EN)`, and `Road`.
    - Validate that `Stop ID` is unique before saving.

- **Story ID: US-07**
  - **Role (As a):** System Admin
  - **Action (I want to):** update/delete a bus stop
  - **Benefit (So that):** I can manage changes in bus stop names or closures.
  - **Acceptance Criteria (AC):**
    - Update modifies the exact record.
    - Delete removes the record (and triggers `ON DELETE SET NULL` in related mapping tables).

---

## 3. Feature: Route-Stop Mapping

- **Story ID: US-08**
  - **Role (As a):** System Admin
  - **Action (I want to):** assign bus stops to a specific bus line
  - **Benefit (So that):** I can define the exact outbound and return paths of a bus.
  - **Acceptance Criteria (AC):**
    - Select a Bus Line and assign multiple Bus Stops.
    - Specify `Direction` (Outbound/Return) and `Stop Order`.

- **Story ID: US-09**
  - **Role (As a):** System Admin
  - **Action (I want to):** reorder stops within a route
  - **Benefit (So that):** I can easily fix the sequence if a stop is misplaced.
  - **Acceptance Criteria (AC):**
    - Provide a drag-and-drop or input interface to change `stop_order`.
    - Ensure the order updates correctly in `TblRouteStop`.

- **Story ID: US-10**
  - **Role (As a):** System Admin
  - **Action (I want to):** view the full route of a bus line
  - **Benefit (So that):** I can verify the sequence of stops visually.
  - **Acceptance Criteria (AC):**
    - Selecting a bus line shows two lists: Outbound Stops and Return Stops, sorted by `stop_order`.

---

## 4. Feature: YPS Store Management

- **Story ID: US-11**
  - **Role (As a):** System Admin
  - **Action (I want to):** view a list of YPS stores
  - **Benefit (So that):** I can see all ticket and top-up locations.
  - **Acceptance Criteria (AC):**
    - Display a list with `Store Name`, `Category`, `Township`, and `Coordinates`.

- **Story ID: US-12**
  - **Role (As a):** System Admin
  - **Action (I want to):** add/edit a YPS store
  - **Benefit (So that):** I can register new stores with their exact GPS coordinates.
  - **Acceptance Criteria (AC):**
    - Form includes fields for standard data + `Latitude` and `Longitude`.
    - The system must convert Lat/Lon into a PostGIS `Point` geometry when saving to the database.

- **Story ID: US-13**
  - **Role (As a):** System Admin
  - **Action (I want to):** assign 'Nearest Stops' to a YPS store
  - **Benefit (So that):** passengers know which stop to get off at to find the store.
  - **Acceptance Criteria (AC):**
    - Interface to select existing Bus Stops and link them to the YPS Store (`TblYpsStore_NearestStop`).

- **Story ID: US-14**
  - **Role (As a):** System Admin
  - **Action (I want to):** assign 'Serving Bus Lines' to a YPS store
  - **Benefit (So that):** passengers know which buses can take them to the store.
  - **Acceptance Criteria (AC):**
    - Interface to add Bus Numbers to the YPS Store (`TblYpsStore_ServingBusLine`).
