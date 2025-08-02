ESP8266 Xeno-Cyborg Scout Network

Firmware: main.c / .inoGateway & UI: .NET 9.0 MAUI Blazor Hybrid

📜 Scenario Overview

Outpost Vesper has deployed 50 unknown cyborg sensor modules (“XenoCores”), each plugged into an ESP8266 “Scout” node. The Scout must:

Catalog its XenoCore readings

Uplink them to the Cyborg Edge Gateway

The Gateway then:

Bridges incoming telemetry (MQTT or HTTP) into RabbitMQ or SignalR

Persists data in a time-series store

Exposes real-time & historical APIs to a .NET MAUI Blazor Hybrid Command Interface

⚙️ Core Functional Requirements

1. ESP8266 Scout Firmware

Xeno-Module Abstraction

Support up to 50 unknown modules, at minimum: NeuroFlux, PlasmaDensity, BioResonance

Define:

enum XenoType { NEURO, PLASMA, BIO, /* … */ };
struct XenoCore {
    XenoType    type;
    const char* label;
    float     (*read)();
};
// register in: XenoCore modules[50];

Unrecognized modules return NAN and log “Unknown Core” over Serial

Warp-Stable Connectivity

Join the Wi-Fi SSID CyborgNet with exponential back-off

Feed the hardware watchdog (ESP.wdtFeed()) during long loops

XenoTelemetry Uplink

Option A: Publish JSON over MQTT to topic xeno/{scoutId}/telemetry

Option B: POST JSON to https://<gateway>/api/xeno/telemetry

Example Payload:

{
  "scoutId":   "xc-scout-01",
  "timestamp": 1712345600,
  "modules": [
    { "type": "NEURO",  "value": 0.842 },
    { "type": "PLASMA", "value": 3.141 },
    { "type": "BIO",    "value": 0.058 }
  ]
}

Persistent Configuration (SPIFFS)

Store Wi-Fi credentials and Gateway URL in /cfg/xenoconfig.json

Provide a Serial menu to list, select, or override saved networks/endpoints

2. Cyborg Edge Gateway Service

Deployment (Docker Compose)

QuantumQueue (RabbitMQ)

TimeVaultDB (InfluxDB or SQLite TS)

.NET Worker (XenoBridge)

Nginx for TLS termination

Ingestion & Bridging

Consume MQTT or HTTP from Scouts

Publish to xenotelemetry exchange or broadcast via SignalR (XenoHub)

Validation & Storage

XenoBridge handles:

Validation of incoming data

Enrichment with calibration metadata

Persistence to TimeVaultDB

Emission of MediatR events (XenoTelemetryReceived)

3. Real-Time Streams & Commands

SignalR Hub (XenoHub)

Broadcast incoming readings via OnTelemetry(scoutId, type, value)

Accept commands (e.g. CalibrateCore, RebootScout) from UI and forward to Scouts via MQTT/HTTP

RabbitMQ Alternative

Topics: xenotelemetry, xenocommands

Routing keys: scout.{id}

4. .NET MAUI Blazor Hybrid Command Interface

Live Scout Dashboard

<ScoutCard> per Scout node showing status, module count, and last readings

<FluxChart> renders real-time graphs for NeuroFlux, PlasmaDensity, BioResonance

Historical Analysis

Expose REST endpoint: GET /api/xeno/history?scout={id}&from={ts}&to={ts}

UI controls: date-range slider, module-type filters, scout search

Command Center

Buttons for actions like CalibrateCore(coreIndex) or DeployFirmware

Animated toasts for acknowledgments or errors

Advanced Visualization & AI Insights

Heatmap visualizations with Chart.js for module value distributions

Embedded AI/LLM widget for anomaly detection and calibration suggestions

Theming & Accessibility

Light/dark toggle with CSS variables: --cyborg-green, --void-purple, --signal-gold

Responsive layout (collapsible sidebar, mobile menu)

ARIA roles, keyboard navigation, minimum contrast ratio of 4.5:1

🛠 Configuration & Management

Local Data Store: EF Core + SQLite (scout registry, module metadata, user preferences)

Design Patterns: Repository, Unit of Work

Database Migrations: EF Core migrations + scouts.db schema file

🏗 Design & Architecture

Technology: .NET 9.0, C# 12 (file-scoped namespaces, required properties, pattern matching)

Key Patterns: Factory, Strategy, Command, Observer, Mediator/CQRS

Project Structure:

/Firmware (ESP8266 code: wifi.c/h, xenocore.c/h, comm.c/h)

/Gateway (XenoApi, XenoBridge services)

/UI (Blazor MAUI Command Interface)

Containerization: Docker Compose for RabbitMQ, TimeVaultDB (optionally k3d)

Security: TLS, JWT/API tokens, RabbitMQ ACLs, encrypted SPIFFS configuration

🚀 Getting Started

Scaffold ESP8266 firmware: Wi-Fi, SPIFFS, XenoCore readings, telemetry uplink

Scaffold .NET MAUI Blazor Hybrid UI: SignalR client, EF Core, initial pages

Build Edge Gateway: ingest telemetry, bridge to RabbitMQ/SignalR, persist data

Create UI components: <ScoutCard>, <FluxChart>, <CommandPanel>

Run locally with Docker Compose (RabbitMQ & TimeVaultDB)

📦 Deliverables

ESP8266 firmware source (main.c/.ino) + build configuration

.NET solution containing /Gateway and /UI projects

Docker Compose files for RabbitMQ and TimeVaultDB

EF Core migration scripts and scouts.db schema

README with architecture diagram, setup instructions, and design patterns

Demonstration video showcasing live telemetry, heatmaps, and AI insights

### INSTRUCTIONS ###
1. Set up appsettings.json in both projects
2. Migrate DB to SQL Server with dotnet EF
3. docker compose up -d
4. If rabbitmq isn't listening on port 1883 you may need to run the following:
     docker exec -it rabbitmq rabbitmq-plugins enable rabbitmq_mqtt
