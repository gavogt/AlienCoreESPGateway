# 🤖 ESP8266 Xeno-Cyborg Scout Network

## Overview

The **Xeno-Cyborg Scout Network** connects 50+ **ESP8266 Scout Nodes** to a secure **Cyborg Edge Gateway**.  
Each scout reads from multiple alien **XenoCore** sensor modules and transmits telemetry to the gateway for real-time monitoring, historical analysis, and remote commands.

---

## 📜 Scenario

**Outpost Vesper** has deployed an array of unknown cyborg sensor modules (“XenoCores”), each connected to an ESP8266 “Scout” node.

The scouts:
1. Detect and catalog their connected XenoCores
2. Transmit readings to the Cyborg Edge Gateway over Wi-Fi

The gateway:
1. Ingests telemetry via **MQTT** or **HTTP**
2. Routes messages into **RabbitMQ** or broadcasts via **SignalR**
3. Persists historical data to **TimeVaultDB** (InfluxDB or SQLite TS)
4. Serves a **.NET 9.0 MAUI Blazor Hybrid Command Interface** for operators

---

## ⚙️ Features

### ESP8266 Scout Firmware
- **XenoCore Abstraction**
  ```c
  enum XenoType { NEURO, PLASMA, BIO };
  struct XenoCore { XenoType type; const char* label; float (*read)(); };

    Supports NeuroFlux, PlasmaDensity, BioResonance (more can be added)

    Unknown modules log “Unknown Core” over Serial

    Warp-Stable Connectivity

        Connect to CyborgNet SSID with exponential back-off

        Feed hardware watchdog to prevent hangs

    Telemetry Uplink

        Option A: Publish JSON over MQTT → xeno/{scoutId}/telemetry

        Option B: POST JSON to /api/xeno/telemetry

    Persistent Config (SPIFFS)

        Store Wi-Fi + Gateway config in /cfg/xenoconfig.json

        Serial menu for listing/changing configs

Cyborg Edge Gateway Service

    Deployment: Docker Compose (RabbitMQ, TimeVaultDB, Nginx, .NET Worker)

    Ingestion & Bridging

        Consume telemetry via MQTT/HTTP

        Publish to xenotelemetry exchange or broadcast via SignalR Hub

    Validation & Storage

        Enrich with calibration metadata

        Persist to InfluxDB or SQLite TS

        Raise XenoTelemetryReceived events for processing

Real-Time Streams & Commands

    SignalR Hub (XenoHub)

        Broadcast telemetry via OnTelemetry(scoutId, type, value)

        Accept commands like CalibrateCore, RebootScout

    RabbitMQ

        Topics: xenotelemetry, xenocommands

        Routing keys: scout.{id}

.NET MAUI Blazor Hybrid Command Interface

    Live Scout Dashboard: status, module counts, last readings

    Historical Analysis: query by date range, module type, scout ID

    Command Center: send calibrations or firmware updates

    Advanced Visualization: Chart.js heatmaps, AI-driven anomaly detection

    Theming & Accessibility:

        Light/dark theme with --cyborg-green, --void-purple, --signal-gold

        Responsive + ARIA roles, keyboard navigation

🛠 Configuration & Management

    Local DB: EF Core + SQLite for scout registry, metadata, user prefs

    Design Patterns: Repository, Unit of Work, Factory, Strategy, Command, Observer, Mediator/CQRS

    Security: TLS, JWT tokens, RabbitMQ ACLs, encrypted SPIFFS config

🏗 Architecture

Technology Stack:

    Firmware: ESP8266 (C/C++)

    Gateway: .NET 9.0, C# 12, EF Core

    Broker: RabbitMQ (with MQTT plugin)

    Time-Series Store: InfluxDB / SQLite TS

    UI: .NET MAUI Blazor Hybrid

    Containerization: Docker Compose

## 🏗 Project Structure

/Firmware # ESP8266 code (wifi.c/h, xenocore.c/h, comm.c/h)
/Gateway # XenoApi, XenoBridge services
/UI # Blazor MAUI Command Interface


---

## 🚀 Getting Started

### 1. Firmware
1. Flash ESP8266 firmware in `/Firmware`.
2. Configure `/cfg/xenoconfig.json` with:
   ```json
   {
     "wifi_ssid": "CyborgNet",
     "wifi_pass": "********",
     "gateway_url": "http://<server-ip>:5000"
   }

2. Gateway + Broker

docker compose up -d

Enable MQTT in RabbitMQ:

docker exec -it rabbitmq rabbitmq-plugins enable rabbitmq_mqtt

3. Database

dotnet ef database update

4. UI

    Build and run the .NET MAUI Blazor Hybrid project.

    Access the dashboard via the configured URL.

📦 Deliverables

ESP8266 firmware source

.NET solution (/Gateway + /UI)

Docker Compose for RabbitMQ + TimeVaultDB

EF Core migration scripts

README + architecture diagram

    Demo video showcasing live telemetry, heatmaps, and AI insights


---
