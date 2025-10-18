---
title: task description for agent
date: '2025-10-18'

author:
  full_name: Jan Ritt
  github:
    username: IxI-Enki
    url: 'https://github.com/IxI-Enki'

agent:
  model: 'Claude 4.5 Sonnet thinking'
  provider: Anthropic
  integrated_development_environment: Cursor
---

<!-- markdownlint-disable MD033 -->

## Instruction for 🤖 Agent

- **Take a look at:**

  - [assignment](files/angabe/angabe_moodle_wmc_05_middleware.md)

  - [scriptum](files/angabe/scriptum_wmc_05_jan.md)

  - [exercise project](clean_architecture_05_validation_with_middleware)

  - [original reference project without any changes](<files/angabe/01_5_CleanArchitecture Validation mit Middleware - Template>)

---

- **Create a task list for yourself below**

  - [ ] **Task 01** - Analyse Assignment & Scriptum:
    > - **Description:** Anforderungen und Patterns aus Assignment und Scriptum im Detail verstehen
    > - **Expected Result:** Vollständiges Verständnis der Clean Architecture, CQRS, MediatR, FluentValidation, ValidationBehavior, Middleware-Pipeline, Layer-Trennung (Domain/Application/Infrastructure/API)
    > - **Notes:** Patterns identifiziert: Clean Architecture mit Layer-Trennung, CQRS (Commands/Queries getrennt), MediatR (Mediator Pattern), FluentValidation (Application-Layer), Domain Validation (Specifications), ValidationBehavior (Pipeline), ValidationExceptionMiddleware (API-Layer), Result<T> Pattern. → User-Validierung erforderlich vor nächstem Task.

  - [ ] **Task 02** - Codebase-Struktur analysieren:
    > - **Description:** Existierende Patterns & Code-Strukturen im Projekt identifizieren
    > - **Expected Result:** Detailliertes Verständnis vorhandener Implementierungen (Commands, Queries, Validators, Repository-Methoden, Controller-Endpoints) um exakt das gleiche Pattern zu verwenden
    > - **Notes:** CreateMeasurementCommand/Handler/Validator existiert, GetAllMeasurements/GetMeasurementById Queries vorhanden, SensorSpecifications/MeasurementSpecifications für Domain-Validierung, Repository Pattern mit Generic- und spezifischen Repositories. → Muss diese Patterns 1:1 übernehmen.

  - [ ] **Task 03** - AddMeasurement Command implementieren:
    > - **Description:** Command mit Location/Name/Timestamp/Value + Auto-Creation von Sensor falls nicht vorhanden
    > - **Expected Result:** Funktionierender AddMeasurementCommand + Handler nach bestehendem Pattern
    > - **Notes:** Handler-Logik: 1) Sensor suchen (Location+Name), 2) Falls nicht vorhanden: neuen Sensor erstellen + SaveChanges für FK, 3) Measurement erstellen, 4) SaveChanges, 5) Result<GetMeasurementDto> zurück. Domain-Validierung greift automatisch.

  - [ ] **Task 04** - AddMeasurement Validation implementieren:
    > - **Description:** FluentValidation Validator für AddMeasurementCommand - Timestamp innerhalb letzter Stunde
    > - **Expected Result:** Validator prüft: Timestamp >= UtcNow.AddHours(-1) && Timestamp <= UtcNow
    > - **Notes:** Application-Layer Validierung (Use-Case spezifisch), Location/Name erfolgt in Domain-Layer, ValidationBehavior führt automatisch aus, bei Fehler: ValidationException → 400 BadRequest via Middleware.

  - [ ] **Task 05** - GetLast100Measurements Query implementieren:
    > - **Description:** Query für 100 neueste Messwerte, sortiert nach Timestamp DESC
    > - **Expected Result:** GetLast100MeasurementsQuery + Handler nach Query-Pattern
    > - **Notes:** Repository mit OrderByDescending(m => m.Timestamp).Take(100), Mapster für DTO-Mapping, IRequest<IReadOnlyCollection<GetMeasurementDto>>.

  - [ ] **Task 06** - API Endpoints bereitstellen:
    > - **Description:** MeasurementsController erweitern: POST /api/measurements + GET /api/measurements/last100
    > - **Expected Result:** Beide Endpoints funktionieren, Controller bleibt schlank (nur Mediator.Send)
    > - **Notes:** POST → AddMeasurementCommand → Result.ToActionResult(), GET → GetLast100MeasurementsQuery → Ok(measurements), ProducesResponseType für Swagger.

  - [ ] **Task 07** - Validierung testen:
    > - **Description:** Domain-, Application-, API-Layer Validierungen manuell prüfen
    > - **Expected Result:** Alle Validierungen greifen korrekt, Exceptions → 400 BadRequest, strukturierte Fehlermeldungen
    > - **Notes:** Testfälle: Name zu kurz, Name=Location, Timestamp Zukunft/zu alt, gültige Daten. Tools: Swagger UI, Api.http.

  - [ ] **Task 08** - Integration testen:
    > - **Description:** End-to-End Test AddMeasurement & GetLast100Measurements
    > - **Expected Result:** Sensor-Auto-Creation funktioniert, max. 100 Messwerte, korrekte Sortierung, alle HTTP-Codes korrekt
    > - **Notes:** Szenarien: 1) Neue Daten, 2) Sensor-Auto-Creation, 3) Bestehender Sensor, 4) GetLast100. Tools: Api.http, Swagger UI.

---
  
❗ Assignment has to be implemented in the [exercise project](clean_architecture_05_validation_with_middleware)

- 🛑 **Do not deviate from given patterns and code structure** → *strictly adhere to patterns & structures of given project/scriptum* ❗

- 🛑 **Do not implement new patterns or code structures** → *strictly adhere to patterns & structures of given project/scriptum* ❗

- 🛑 **Do not implement validations in wrong layers** → *strictly adhere to layers of given project/scriptum* ❗
