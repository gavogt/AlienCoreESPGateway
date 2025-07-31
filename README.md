/*
 * Assignment: ESP8266 Xeno-Cyborg Scout Network – main.c & .NET 9.0 MAUI Blazor Hybrid
 *
 * Scenario Overview
 * -----------------
 * Outpost Vesper has deployed a squadron of 50 unknown cyborg sensor modules (“XenoCores”).
 * Each XenoCore plugs into an ESP8266 “Scout” node that must:
 *   1. Catalog its readings
 *   2. Uplink them to the Cyborg Edge Gateway
 *
 * The gateway:
 *   • Bridges incoming XenoTelemetry (via MQTT or HTTP) into RabbitMQ or SignalR streams
 *   • Persists data
 *   • Exposes real-time and historical APIs for the .NET MAUI Blazor Hybrid Command Interface
 *
 * Core Functional Requirements
 * ----------------------------
 *
 * 1. ESP8266 Scout Firmware
 *    ● Xeno-Module Abstraction
 *      – Support up to 50 unknown XenoCores; design for at least three test types
 *        (e.g., “NeuroFlux”, “PlasmaDensity”, “BioResonance”)
 *      – Define:
 *          enum XenoType { NEURO, PLASMA, BIO, … };
 *      – Create:
 *          struct XenoCore {
 *              XenoType    type;
 *              const char* label;
 *              float (*read)();
 *          };
 *        and register in `XenoCore modules[50]`.
 *      – Unrecognized modules must return NAN and log “Unknown Core” over Serial
 *
 *    ● Warp-Stable Connectivity
 *      – Join the “CyborgNet” SSID with exponential back-off
 *      – Feed the hardware watchdog (`ESP.wdtFeed()`) during long loops
 *
 *    ● XenoTelemetry Uplink
 *      – Option A: Publish JSON over MQTT to topic `xeno/{scoutId}/telemetry`
 *      – Option B: POST JSON to `https://<gateway>/api/xeno/telemetry` via ArduinoHttpClient
 *
 *      JSON Payload Format:
 *      {
 *        "scoutId":   "xc-scout-01",
 *        "timestamp": 1712345600,
 *        "modules": [
 *          { "type": "NEURO", "value": 0.842 },
 *          { "type": "PLASMA","value": 3.141 },
 *          { "type": "BIO",   "value": 0.058 }
 *        ]
 *      }
 *
 *    ● Persistent Configuration (SPIFFS)
 *      – Store “CyborgNet” credentials and gateway URL in `/cfg/xenoconfig.json`
 *      – Provide a Serial-menu to list, select, or override saved networks and endpoints
 *
 * 2. Cyborg Edge Gateway Service
 *    ● Deployment (Docker-Compose stack)
 *      – QuantumQueue (RabbitMQ)
 *      – TimeVaultDB (InfluxDB or SQLite TS)
 *      – .NET Worker (XenoBridge)
 *      – Nginx for TLS termination
 *
 *    ● Ingestion & Bridging
 *      – Consume MQTT or HTTP posts from scouts
 *      – Publish into `xenotelemetry` exchange or broadcast via XenoHub (SignalR)
 *
 *    ● Validation & Storage
 *      – XenoBridge subscribes to queues, validates cosmic signatures, enriches with metadata
 *      – Persist to TimeVaultDB; emit MediatR events (`XenoTelemetryReceived`)
 *
 * 3. Real-Time Streams & Cyborg Commands
 *    ● SignalR Hub (XenoHub)
 *      – Broadcast incoming readings (`OnTelemetry(scoutId,type,value)`) to UI clients
 *      – Accept Commands (`CalibrateCore`, `RebootScout`) and forward to scouts via MQTT/HTTP
 *
 *    ● RabbitMQ Alternative
 *      – Define topics: “xenotelemetry” and “xenocommands”; use routing keys `scout.{id}`
 *
 * 4. .NET MAUI Blazor Hybrid Command Interface
 *    ● Live Scout Dashboard
 *      – `<ScoutCard>` per ESP node: status, module count, latest readings
 *      – `<FluxChart>` real-time graphs of NeuroFlux, PlasmaDensity, BioResonance
 *
 *    ● Historical Analysis
 *      – REST: `GET /api/xeno/history?scout={id}&from={ts}&to={ts}`
 *      – UI: date-range slider, module-type filters, scout search
 *
 *    ● Command Center
 *      – Controls for `CalibrateCore(coreIndex)` or `DeployFirmware`
 *      – Animated toasts for ACKs/errors
 *
 *    ● Advanced Visualization & AI Insights
 *      – Heatmap visualizations with Chart.js showing value distributions
 *      – Embedded LLM widget for anomaly detection and calibration suggestions
 *
 *    ● Theming & Accessibility
 *      – Light/dark toggle with CSS vars (`--cyborg-green`, `--void-purple`, `--signal-gold`)
 *      – Responsive: collapsible sidebar, mobile hamburger menu
 *      – ARIA roles, keyboard navigation, contrast ≥4.5:1
 *
 * Configuration & Management
 * --------------------------
 *  • Local Data Store: EF Core + SQLite (scout registry, module metadata, user prefs)
 *  • Patterns: Repository, Unit-of-Work
 *  • Migrations & Schema: EF Core migrations + `scouts.db` schema file
 *
 * Design & Architecture Guidelines
 * --------------------------------
 *  • .NET 9.0 + C# 12: file-scoped namespaces, required props, pattern matching
 *  • Key Patterns: Factory, Strategy, Command, Observer, Mediator/CQRS, Repository
 *  • Modular Structure:
 *      /Firmware  (wifi.c/h, xenocore.c/h, comm.c/h)
 *      /Gateway   (XenoApi, XenoBridge)
 *      /UI        (CommandInterface)
 *  • Containerization: Docker-Compose (RabbitMQ, TimeVaultDB), optional k3d
 *  • Security: TLS, JWT/API tokens, RabbitMQ ACLs, encrypted SPIFFS
 *
 * Getting Started
 * ---------------
 * 1. Scaffold ESP8266 firmware: Wi-Fi + SPIFFS + XenoCore reads + telemetry uplink
 * 2. Scaffold .NET MAUI Blazor Hybrid: add SignalR client, EF Core, initial UI
 * 3. Build Edge Gateway: ingest, bridge, persist
 * 4. Create UI components: `<ScoutCard>`, `<FluxChart>`, `<CommandPanel>`
 * 5. Containerize services for local development
 *
 * Deliverables
 * ------------
 *  • ESP8266 firmware source (main.c/.ino) + build config
 *  • .NET solution with /Gateway & /UI projects
 *  • Docker-Compose for RabbitMQ & TimeVaultDB
 *  • EF Core migrations & scouts.db
 *  • README with architecture diagram, setup steps, pattern mapping
 *  • Demo video showcasing telemetry, heatmaps, and AI insights
 */

### INSTRUCTIONS ###
1. Set up appsettings.json in both projects
2. docker compose up -d
3. If rabbitmq isn't listening on port 1883 you may need to run the following:
     docker exec -it rabbitmq rabbitmq-plugins enable rabbitmq_mqtt
