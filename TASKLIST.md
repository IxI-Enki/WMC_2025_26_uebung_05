<!-- markdownlint-disable MD022 -->
<!-- markdownlint-disable MD031 -->
<!-- markdownlint-disable MD032 -->
<!-- markdownlint-disable MD033 -->
<!-- markdownlint-disable MD034 -->
<!-- markdownlint-disable MD036 -->
<!-- markdownlint-disable MD040 -->

# Task List - WMC Übung 05: Middleware & Validation

## Aufgabenstellung
Implementieren Sie **AddMeasurement** und **GetLast100Measurements** gemäß den Anforderungen des Assignments unter strikter Einhaltung der bestehenden Clean Architecture Patterns.

---

## Tasks

### ✅ Task 01 - Analyse Assignment & Scriptum
**Status:** Completed
**Beschreibung:** Anforderungen und Patterns aus Assignment und Scriptum im Detail verstehen

**Expected Result:**
- Vollständiges Verständnis der Clean Architecture
- CQRS Pattern, MediatR, FluentValidation verstanden
- ValidationBehavior und Middleware-Pipeline klar
- Layer-Trennung (Domain/Application/Infrastructure/API) verstanden

**Notes - Identifizierte Patterns & Strukturen:**
- **Clean Architecture** mit strikter Layer-Trennung
- **CQRS:** Commands (Schreiboperationen) und Queries (Leseoperationen) getrennt
- **MediatR:** Mediator Pattern für lose Kopplung
- **FluentValidation:** Application-Layer Validierung für Use-Case spezifische Regeln
- **Domain Validation:** Entity-interne Validierung via Specifications
- **ValidationBehavior:** Pipeline für automatische Validierung vor Command/Query Handler
- **ValidationExceptionMiddleware:** Zentrale Exception-Behandlung in API-Layer
- **Result<T> Pattern:** Einheitliche Rückgabewerte für Commands
- **User-Validierung erforderlich:** Verständnis mit User abgleichen bevor weiter

---

### ✅ Task 02 - Codebase-Struktur analysieren
**Status:** Completed
**Beschreibung:** Existierende Patterns & Code-Strukturen im Projekt identifizieren und verstehen

**Expected Result:**
- Detailliertes Verständnis vorhandener Implementierungen
- Commands, Queries, Validators, Repository-Methoden verstanden
- Controller-Endpoints Struktur klar
- Kann exakt das gleiche Pattern für neue Features verwenden

**Notes - Vorhandene Strukturen:**
```
Application/
├── Features/
│   ├── Measurements/
│   │   ├── Commands/
│   │   │   ├── CreateMeasurement/
│   │   │   │   ├── CreateMeasurementCommand.cs
│   │   │   │   ├── CreateMeasurementCommandHandler.cs
│   │   │   │   └── CreateMeasurementCommandValidator.cs
│   │   │   └── DeleteMeasurement/
│   │   └── Queries/
│   │       ├── GetAllMeasurements/
│   │       └── GetMeasurementById/
│   └── Sensors/
│       ├── Commands/ (CreateSensor, UpdateSensor, DeleteSensor)
│       └── Queries/ (GetAllSensors, GetSensorById, GetSensorsWithCounts)
├── Pipeline/
│   └── ValidationBehavior.cs
└── Services/
    └── SensorUniquenessChecker.cs

Domain/
├── Specifications/
│   ├── SensorSpecifications.cs
│   └── MeasurementSpecifications.cs
└── Exceptions/
    └── DomainValidationException.cs

Api/
├── Controllers/
│   ├── MeasurementsController.cs
│   └── SensorsController.cs
├── Middleware/
│   └── ValidationExceptionMiddleware.cs
└── Extensions/
    └── ResultExtensions.cs
```

**Wichtig:** Muss diese Patterns 1:1 übernehmen für neue Features!

---

### ✅ Task 03 - AddMeasurement Command implementieren
**Status:** Completed
**Beschreibung:** AddMeasurement Command mit Location/Name/Timestamp/Value implementieren, inklusive Auto-Creation von Sensor falls nicht vorhanden

**Expected Result:**
- Funktionierender `AddMeasurementCommand` (analog zu `CreateMeasurementCommand`)
- Funktionierender `AddMeasurementCommandHandler`
- Pattern exakt wie bestehende Commands

**Implementation Details:**

**Command-Properties:**
```csharp
public record AddMeasurementCommand(
    string Location,
    string Name,
    DateTime Timestamp,
    double Value
) : IRequest<Result<GetMeasurementDto>>;
```

**Handler-Logik:**
1. Sensor per `Location + Name` suchen via `uow.Sensors.GetByLocationAndNameAsync()`
2. Falls nicht vorhanden:
   - Neuen `Sensor(location, name)` erstellen (Domain-Validierung greift automatisch)
   - `await uow.Sensors.AddAsync(sensor, ct)`
   - `await uow.SaveChangesAsync(ct)` - **wichtig für FK!**
3. `new Measurement(sensor, value, timestamp)` erstellen
4. `await uow.Measurements.AddAsync(measurement, ct)`
5. `await uow.SaveChangesAsync(ct)`
6. `return Result<GetMeasurementDto>.Created(measurement.Adapt<GetMeasurementDto>())`

**Notes:**
- Domain-Validierung greift automatisch beim Sensor-/Measurement-Erstellen
- Muss `IRequest<Result<GetMeasurementDto>>` implementieren
- Sensor muss VOR Measurement gespeichert werden für FK-Generation

---

### ✅ Task 04 - AddMeasurement Validation implementieren
**Status:** Completed
**Beschreibung:** FluentValidation Validator für AddMeasurementCommand (Application-Layer) - Timestamp innerhalb letzter Stunde prüfen

**Expected Result:**
- `AddMeasurementCommandValidator` prüft korrekt
- Timestamp-Validierung: `Timestamp >= DateTime.UtcNow.AddHours(-1) && Timestamp <= DateTime.UtcNow`
- ValidationBehavior führt Validator automatisch aus

**Implementation:**
```csharp
public class AddMeasurementCommandValidator : AbstractValidator<AddMeasurementCommand>
{
    public AddMeasurementCommandValidator()
    {
        RuleFor(x => x.Timestamp)
            .Must(ts => ts <= DateTime.UtcNow && ts >= DateTime.UtcNow.AddHours(-1))
            .WithMessage("Timestamp must be within the last hour.");
    }
}
```

**Notes:**
- Validierung auf **Application-Layer** (Use-Case spezifisch!)
- Location/Name Validierung erfolgt bereits in **Domain-Layer** (SensorSpecifications)
- ValidationBehavior in MediatR Pipeline führt Validator automatisch aus
- Bei Fehler: `ValidationException` → Middleware wandelt in 400 BadRequest um

---

### ✅ Task 05 - GetLast100Measurements Query implementieren
**Status:** Completed
**Beschreibung:** Query für die 100 neuesten Messwerte, sortiert nach Timestamp absteigend

**Expected Result:**
- `GetLast100MeasurementsQuery` nach bestehendem Query-Pattern
- `GetLast100MeasurementsQueryHandler` funktioniert korrekt
- Liefert maximal 100 Einträge, sortiert nach Timestamp DESC

**Implementation:**

**Query:**
```csharp
public record GetLast100MeasurementsQuery
    : IRequest<IReadOnlyCollection<GetMeasurementDto>>;
```

**QueryHandler:**
```csharp
public class GetLast100MeasurementsQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetLast100MeasurementsQuery, IReadOnlyCollection<GetMeasurementDto>>
{
    public async Task<IReadOnlyCollection<GetMeasurementDto>> Handle(
        GetLast100MeasurementsQuery request,
        CancellationToken ct)
    {
        var measurements = await uow.Measurements
            .GetAllAsync(
                ct,
                orderBy: q => q.OrderByDescending(m => m.Timestamp).Take(100)
            );

        return measurements.Adapt<IReadOnlyCollection<GetMeasurementDto>>();
    }
}
```

**Notes:**
- Repository-Methode mit `OrderByDescending(m => m.Timestamp).Take(100)`
- Mapster für Entity → DTO Mapping
- Validator optional (keine Query-Parameter erforderlich)
- Eventuell neue Repository-Methode `GetLast100MeasurementsAsync()` in `IMeasurementRepository`

---

### ✅ Task 06 - API Endpoints bereitstellen
**Status:** Completed
**Beschreibung:** MeasurementsController erweitern mit AddMeasurement (POST) und GetLast100Measurements (GET) Endpoints

**Expected Result:**
- **POST** `/api/measurements` → AddMeasurement
- **GET** `/api/measurements/last100` → GetLast100Measurements
- Controller bleibt schlank (nur Mediator.Send)
- Korrekte HTTP-Statuscodes

**Implementation:**

**AddMeasurement Endpoint:**
```csharp
[HttpPost]
[ProducesResponseType(typeof(GetMeasurementDto), StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public async Task<IActionResult> AddMeasurement(
    [FromBody] AddMeasurementCommand command,
    CancellationToken ct)
{
    var result = await _mediator.Send(command, ct);

    return result.ToActionResult(
        this,
        createdAtAction: nameof(GetById),
        routeValues: new { id = result?.Value?.Id }
    );
}
```

**GetLast100Measurements Endpoint:**
```csharp
[HttpGet("last100")]
[ProducesResponseType(typeof(IReadOnlyCollection<GetMeasurementDto>), StatusCodes.Status200OK)]
public async Task<IActionResult> GetLast100Measurements(CancellationToken ct)
{
    var measurements = await _mediator.Send(new GetLast100MeasurementsQuery(), ct);

    return Ok(measurements);
}
```

**Notes:**
- Controller bleibt schlank (nur `Mediator.Send` + `Result.ToActionResult`)
- `ProducesResponseType` Attribute für Swagger-Dokumentation
- `ResultExtensions` für einheitliche HTTP-Responses verwenden
- `CreatedAtAction` für POST mit Location-Header

---

### ✅ Task 07 - Validierung testen
**Status:** Completed
**Beschreibung:** Domain-, Application- und API-Layer Validierungen manuell prüfen

**Expected Result:**
- Domain-Validierung wirft `DomainValidationException` bei ungültigen Daten
- Application-Validierung wirft `ValidationException` bei Timestamp außerhalb letzter Stunde
- Middleware wandelt beide Exceptions korrekt in **400 BadRequest** um
- Strukturierte Fehlermeldungen in Response

**Testfälle:**

| Test | Input | Expected Result |
|------|-------|-----------------|
| **1. Name zu kurz** | Name = "a" | 400 BadRequest - "Name muss mindestens 2 Zeichen haben." |
| **2. Name = Location** | Location = "Test", Name = "Test" | 400 BadRequest - "Name darf nicht der Location entsprechen." |
| **3. Timestamp in Zukunft** | Timestamp = DateTime.UtcNow.AddHours(1) | 400 BadRequest - "Timestamp must be within the last hour." |
| **4. Timestamp > 1h alt** | Timestamp = DateTime.UtcNow.AddHours(-2) | 400 BadRequest - "Timestamp must be within the last hour." |
| **5. Gültige Daten** | Valid Location, Name, Timestamp, Value | 201 Created mit GetMeasurementDto |

**Tools:**
- Swagger UI (https://localhost:7085/swagger)
- Api.http
- Postman (optional)

---

### ✅ Task 08 - Integration testen
**Status:** Completed
**Beschreibung:** End-to-End Test der neuen Features AddMeasurement & GetLast100Measurements

**Expected Result:**
- AddMeasurement erstellt Sensor falls nicht vorhanden
- AddMeasurement speichert Measurement korrekt
- GetLast100Measurements liefert max. 100 Messwerte
- Korrekte Sortierung (neueste zuerst)
- Alle HTTP-Statuscodes korrekt

**Testszenarien:**

**Szenario 1: Positiv-Test (gültige Daten)**
```json
POST /api/measurements
{
  "location": "Wohnzimmer",
  "name": "Temperatur",
  "timestamp": "2025-10-18T10:00:00Z",
  "value": 22.5
}
```
**Expected:** 201 Created mit GetMeasurementDto

**Szenario 2: Sensor-Auto-Creation**
```json
POST /api/measurements
{
  "location": "Neuer Ort",
  "name": "Neuer Sensor",
  "timestamp": "2025-10-18T10:00:00Z",
  "value": 15.0
}
```
**Expected:**
- Sensor wird angelegt
- Measurement wird gespeichert
- 201 Created

**Szenario 3: Bestehender Sensor**
```json
POST /api/measurements (gleicher Location/Name wie Szenario 2)
{
  "location": "Neuer Ort",
  "name": "Neuer Sensor",
  "timestamp": "2025-10-18T10:05:00Z",
  "value": 16.0
}
```
**Expected:**
- Sensor wird NICHT neu angelegt
- Nur Measurement wird gespeichert
- 201 Created

**Szenario 4: GetLast100Measurements**
```
GET /api/measurements/last100
```
**Expected:**
- Max. 100 Einträge
- Sortiert nach Timestamp DESC (neueste zuerst)
- 200 OK

**Tools:**
- Api.http
- Swagger UI
- Manueller Test via Browser/Postman

---

### ✅ Task 09 - DomainValidationResult Klasse erstellen
**Status:** Completed
**Beschreibung:** DomainValidationResult in Domain/Common erstellen gemäß Template-Pattern aus Scriptum Sektion 43

**Expected Result:**
- DomainValidationResult record mit Factory-Methoden Success() und Failure()
- Property-basierte Validierung (IsValid, Property, ErrorMessage)
- Lightweight für Domain-Layer Validierungen

**Implementation:**
```csharp
namespace Domain.Common;

/// <summary>
/// Lightweight domain validation result with factory helpers.
/// </summary>
public sealed record DomainValidationResult(bool IsValid, string Property, string? ErrorMessage)
{
    public static DomainValidationResult Success(string property) => new(true, property, null);
    public static DomainValidationResult Failure(string property, string message) => new(false, property, message);
}
```

**Notes:**
- Template Pattern aus Scriptum Sektion 43
- Wird von SensorSpecifications.CheckLocation(), CheckName(), CheckNameNotEqualLocation() verwendet
- ✅ Bereits im Projekt vorhanden

---

### ✅ Task 10 - SensorSpecifications erweitern mit ValidateEntityInternal
**Status:** Completed
**Beschreibung:** SensorSpecifications um CheckLocation(), CheckName(), CheckNameNotEqualLocation() und ValidateEntityInternal() erweitern gemäß Scriptum Sektion 44, 64, 65

**Expected Result:**
- CheckLocation() validiert: Location nicht leer
- CheckName() validiert: Name nicht leer + Mindestlänge 2
- CheckNameNotEqualLocation() validiert: Name ≠ Location
- ValidateEntityInternal() führt alle Checks aus, wirft DomainValidationException bei erstem Fehler

**Implementation:**
```csharp
public static class SensorSpecifications
{
    public const int NameMinLength = 2;

    public static DomainValidationResult CheckLocation(string location) =>
        string.IsNullOrWhiteSpace(location)
            ? DomainValidationResult.Failure("Location", "Location darf nicht leer sein.")
            : DomainValidationResult.Success("Location");

    public static DomainValidationResult CheckName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return DomainValidationResult.Failure("Name", "Name darf nicht leer sein.");
        if (name.Trim().Length < NameMinLength)
            return DomainValidationResult.Failure("Name", $"Name muss mindestens {NameMinLength} Zeichen haben.");
        return DomainValidationResult.Success("Name");
    }

    public static DomainValidationResult CheckNameNotEqualLocation(string name, string location) =>
        string.Equals(name.Trim(), location.Trim(), StringComparison.OrdinalIgnoreCase)
            ? DomainValidationResult.Failure("Name", "Name darf nicht der Location entsprechen.")
            : DomainValidationResult.Success("Name");

    public static void ValidateEntityInternal(string location, string name)
    {
        var validationResults = new List<DomainValidationResult>
        {
            CheckLocation(location),
            CheckName(name),
            CheckNameNotEqualLocation(name, location)
        };
        foreach (var result in validationResults)
        {
            if (!result.IsValid)
                throw new DomainValidationException(result.Property, result.ErrorMessage!);
        }
    }

    public static async Task ValidateEntityExternal(int id, string location, string name,
        ISensorUniquenessChecker uniquenessChecker, CancellationToken ct = default)
    {
        if (!await uniquenessChecker.IsUniqueAsync(id, location, name, ct))
            throw new DomainValidationException("Sensor", "Ein Sensor mit der gleichen Location und dem gleichen Namen existiert bereits.");
    }
}
```

**Notes:**
- Pattern aus Scriptum Sektion 44 (Domain-Rules zentral verwalten), 64 (Specifications), 65 (Internal und External)
- ValidateEntityInternal = Domain-interne Regeln (ohne DB-Zugriff)
- ValidateEntityExternal = Domain-übergreifende Regeln (mit DB-Zugriff für Uniqueness)
- ✅ Implementiert

---

### ✅ Task 11 - Sensor CreateAsync/UpdateAsync um ValidateEntityInternal erweitern
**Status:** Completed
**Beschreibung:** Sensor.CreateAsync() und UpdateAsync() müssen ValidateEntityInternal() aufrufen ZUSÄTZLICH zu ValidateEntityExternal()

**Expected Result:**
- CreateAsync() führt ValidateEntityInternal() DANN ValidateEntityExternal() aus
- UpdateAsync() führt ValidateEntityInternal() DANN ValidateEntityExternal() aus
- Domain-Layer garantiert korrekte Daten sowohl für Format als auch Uniqueness

**Implementation:**

**Sensor.CreateAsync():**
```csharp
public static async Task<Sensor> CreateAsync(string location, string name,
    ISensorUniquenessChecker uniquenessChecker, CancellationToken ct = default)
{
    var trimmedLocation = (location ?? string.Empty).Trim();
    var trimmedName = (name ?? string.Empty).Trim();

    // Domain-Validierung: Format + Uniqueness
    SensorSpecifications.ValidateEntityInternal(trimmedLocation, trimmedName);
    await SensorSpecifications.ValidateEntityExternal(0, trimmedLocation, trimmedName, uniquenessChecker, ct);

    return new Sensor(trimmedLocation, trimmedName);
}
```

**Sensor.UpdateAsync():**
```csharp
public async Task UpdateAsync(string location, string name,
    ISensorUniquenessChecker uniquenessChecker, CancellationToken ct = default)
{
    var trimmedLocation = (location ?? string.Empty).Trim();
    var trimmedName = (name ?? string.Empty).Trim();

    // Domain-Validierung: Format + Uniqueness
    SensorSpecifications.ValidateEntityInternal(trimmedLocation, trimmedName);
    await SensorSpecifications.ValidateEntityExternal(Id, trimmedLocation, trimmedName, uniquenessChecker, ct);

    Location = trimmedLocation;
    Name = trimmedName;
}
```

**Notes:**
- Pattern aus Template-Projekt und Scriptum Sektion 65
- Beide Validierungen (intern + extern) sind DOMAIN-Layer Verantwortung
- Application-Layer macht ZUSÄTZLICH FluentValidation für Use-Case-spezifische Regeln
- ✅ Implementiert

---

## Wichtige Constraints

🛑 **KRITISCH - Patterns einhalten:**
- Nicht von vorgegebenen Patterns abweichen
- Keine neuen Patterns oder Strukturen implementieren
- Validierungen in korrekten Layers implementieren:
  - **Domain:** Entity-interne Regeln (SensorSpecifications, MeasurementSpecifications)
  - **Application:** Use-Case Regeln (FluentValidation - Timestamp innerhalb letzter Stunde)
  - **API:** Input-Format Validierung (automatisch durch ModelState)

🛑 **Layer-Verantwortlichkeiten:**
- **Domain:** Geschäftslogik, Entity-Invarianten
- **Application:** Use-Cases, Orchestrierung, DTO-Validierung
- **Infrastructure:** Persistenz, Datenbank-Zugriff
- **API:** HTTP-Request/Response Handling, Routing

---

## Projektstruktur

```
clean_architecture_05_validation_with_middleware/
├── Domain/
│   ├── Entities/
│   ├── Specifications/
│   ├── Contracts/
│   └── Exceptions/
├── Application/
│   ├── Features/
│   │   ├── Measurements/
│   │   │   ├── Commands/
│   │   │   └── Queries/
│   │   └── Sensors/
│   ├── Pipeline/
│   ├── Interfaces/
│   └── DependencyInjection.cs
├── Infrastructure/
│   ├── Persistence/
│   └── Services/
└── Api/
    ├── Controllers/
    ├── Middleware/
    └── Extensions/
```

---

## Status Overview

| Task # | Task Name | Status | Progress |
|--------|-----------|--------|----------|
| 01 | Analyse Assignment & Scriptum | ✅ Completed | 100% |
| 02 | Codebase-Struktur analysieren | ✅ Completed | 100% |
| 03 | AddMeasurement Command | ✅ Completed | 100% |
| 04 | AddMeasurement Validation | ✅ Completed | 100% |
| 05 | GetLast100Measurements Query | ✅ Completed | 100% |
| 06 | API Endpoints | ✅ Completed | 100% |
| 07 | Validierung testen | ✅ Completed | 100% |
| 08 | Integration testen | ✅ Completed | 100% |
| 09 | DomainValidationResult erstellen | ✅ Completed | 100% |
| 10 | SensorSpecifications erweitern | ✅ Completed | 100% |
| 11 | Sensor CreateAsync/UpdateAsync erweitern | ✅ Completed | 100% |

---

**Erstellt:** 2025-10-18
**Agent:** Claude 4.5 Sonnet
**Repository:** https://github.com/IxI-Enki/WMC_2025_26_uebung_05
**Branch:** feat/implementation/claude-2025-10-18
