# CourierService

**Overview**

CourierService is a small sample application that calculates delivery costs, applies promotional offers, and schedules deliveries using a simple vehicle/ETA algorithm. The solution is split into clear layers (API, Application, Domain, Console, Front-end) to demonstrate separation of concerns and testability.

---

## Thought process & assumptions
- I prioritized **clarity and separation of concerns**: Domain entities, Application services (business rules), and API (controllers) are distinct to keep responsibilities small.
- Assumed **no external database**: current offer data is in-memory (see `OfferService`). This keeps the sample focused on algorithms rather than persistence.
- The scheduling algorithm picks optimal shipments for vehicles using a combinatorial selection method (finds best subset per vehicle until all packages are assigned). This is adequate for small input sizes used in the sample.
- Inputs are validated using `Guard` utilities. Invalid or missing inputs will throw explicit errors.

---

## Design decisions
- Layered architecture:
  - `CourierService.Domain` — entities (Package, Vehicle, Offer).
  - `CourierService.Application` — services and interfaces (`ICostCalculator`, `IOfferService`, `IDeliveryScheduler`), DTOs and business logic.
  - `CourierService.API` — controllers and middleware. Error handling is centralized in `ExceptionHandlingMiddleware`.
  - `CourierService.Console` — simple console runner (for manual or CI demos).
- Clear abstractions via interfaces make the services easy to test and swap out implementations.
- Offers are represented with basic eligibility rules (weight & distance ranges) and stored initially in memory to keep the scope focused.

---

## How to run

Prerequisites:
- .NET SDK 8 (or compatible with `net8.0`) installed and on PATH
- Node.js (18+) and npm
- Optional: Angular CLI (`npm i -g @angular/cli`) — not required if using `npm start` in the front-end folder

---

## Sample request — Calculate delivery (POST /api/delivery)

This endpoint accepts a JSON payload with `BaseDeliveryCost`, a list of `Packages`, and an optional `Vehicles` object.

Example payload:

```json
{
  "BaseDeliveryCost": 100.00,
  "Packages": [
    { "Id": "PKG1", "WeightKg": 50,  "DistanceKm": 30,  "OfferCode": "OFR001" },
    { "Id": "PKG2", "WeightKg": 75,  "DistanceKm": 125, "OfferCode": "OFR002" },
    { "Id": "PKG3", "WeightKg": 175, "DistanceKm": 100 }
  ],
  "Vehicles": { "NumberOfVehicles": 2, "MaxSpeedKmH": 70, "MaxCarryWeightKg": 200 }
}
```
The response is a `DeliveryResultDto` object containing calculated per-package costs, discounts, totals, ETA in hours for each package, and `MaxDeliveryTimeHours`.

---

