#include <WiFiS3.h>             // native Wi‑Fi driver for UNO R4 WiFi
#include <ArduinoMqttClient.h>  // official Arduino MQTT client
#include <Arduino_LED_Matrix.h> // LED‑matrix library for R4 WiFi

// ─── Wi‑Fi Credentials ─────────────────────────────────────────────────────────
const char* WIFI_SSID = "Your ssid";
const char* WIFI_PASS = "Your pass";

// ─── MQTT Broker Settings ──────────────────────────────────────────────────────
const char* MQTT_BROKER = "192.168.1.164";
const uint16_t MQTT_PORT = 1883;
const char* MQTT_TOPIC  = "xeno/uno-scout/telemetry";
const char* SCOUT_ID    = "xc-scout-01";

// ─── Sensor & Actuator Pins ────────────────────────────────────────────────────
const uint8_t LASER_PIN     = 2;    // D2 → Laser “S”
const uint8_t SOUND_DIGITAL = 3;    // D3 → Sound “D0”

// ─── XenoCore Abstraction ─────────────────────────────────────────────────────
enum XenoType { NEURO, PLASMA, BIO, UNKNOWN };

float readNeuroFlux() {
  // read NeuroFlux from A0
  return analogRead(A0) * (3.3f / 1023.0f);
}
float readPlasmaDensity() {
  // read PlasmaDensity from A1
  return analogRead(A1) * (3.3f / 1023.0f);
}
float readBioResonance() {
  // simulate BioResonance on A2
  return analogRead(A2) * (3.3f / 1023.0f);
}
float readUnknown() {
  Serial.println(F("Unknown Core"));
  return NAN;
}

struct XenoCore {
  XenoType    type;
  const char* label;
  float     (*read)();
};

// Populate first three, rest default to UNKNOWN
XenoCore modules[50] = {
  { NEURO,  "NEURO",  readNeuroFlux     },
  { PLASMA, "PLASMA", readPlasmaDensity },
  { BIO,    "BIO",    readBioResonance  },
  { UNKNOWN, nullptr, readUnknown }
};
const size_t moduleCount = 3;  // number of real modules

// ─── Alien‑smiley frames (8×12) ───────────────────────────────────────────────
byte smiley0[8][12] = {
  {0,0,0,0,0,0,0,0,0,0,0,0},
  {0,1,1,1,1,1,1,1,1,1,1,0},
  {1,0,0,0,0,0,0,0,0,0,0,1},
  {1,0,0,0,1,0,0,1,0,0,0,1},
  {1,0,0,0,1,1,1,1,0,0,0,1},
  {1,0,0,0,0,0,0,0,0,0,0,1},
  {0,1,1,1,1,1,1,1,1,1,1,0},
  {0,0,0,0,0,0,0,0,0,0,0,0}
};
byte smiley1[8][12] = {
  {0,0,0,0,0,0,0,0,0,0,0,0},
  {0,1,1,1,1,1,1,1,1,1,1,0},
  {1,0,0,0,0,0,0,0,0,0,0,1},
  {1,0,0,0,0,0,0,0,0,0,0,1},
  {1,0,0,0,1,1,1,1,0,0,0,1},
  {1,0,0,0,0,0,0,0,0,0,0,1},
  {0,1,1,1,1,1,1,1,1,1,1,0},
  {0,0,0,0,0,0,0,0,0,0,0,0}
};

// ─── Globals ─────────────────────────────────────────────────────────────────
WiFiClient      network;
MqttClient      mqttClient(network);
ArduinoLEDMatrix matrix;

// ─── Helpers ─────────────────────────────────────────────────────────────────
void connectWiFi() {
  Serial.println(F("🌌 Initiating cosmic Wi‑Fi ritual…"));
  Serial.print(F("🛰️  SSID: “")); Serial.print(WIFI_SSID); Serial.println(F("”"));
  WiFi.begin(WIFI_SSID, WIFI_PASS);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(F(".·°• radiowaves aligning •°·."));
  }
  Serial.println();
  Serial.print(F("✅ Wi‑Fi tethered. IP = "));
  Serial.println(WiFi.localIP());
  Serial.println(F("🔌 Powering up MQTT engines…"));
}

void connectMQTT() {
  mqttClient.setId("UnoR4Client");
  Serial.println(F("🔭 Scanning for MQTT broker…"));
  Serial.print(F("🌐 Broker @ "));
  Serial.print(MQTT_BROKER); Serial.print(':'); Serial.println(MQTT_PORT);
  while (!mqttClient.connect(MQTT_BROKER, MQTT_PORT)) {
    delay(1000);
    Serial.print(F("⚡ Attempting quantum link... ⚡"));
  }
  Serial.println();
  Serial.println(F("🛰️  MQTT warp‑drive engaged!"));
}

// ─── Setup & Loop ─────────────────────────────────────────────────────────────
void setup() {
  Serial.begin(115200);
  while (!Serial) {;}       // for boards that need Serial ready
  matrix.begin();           // init LED matrix

  pinMode(LASER_PIN, OUTPUT);
  pinMode(SOUND_DIGITAL, INPUT);

  Serial.println(F("🚀 UNO‑R4 Scout boot sequence start…"));
  connectWiFi();
  connectMQTT();
  Serial.println(F("🔒 All systems nominal. Entering main loop…"));
}

void loop() {
  // — MQTT keep‑alive —
  if (!mqttClient.connected()) {
    Serial.println(F("⚠️  MQTT link lost. Re‑establishing…"));
    connectMQTT();
  }
  mqttClient.poll();

  // — Build JSON payload —
  unsigned long ts = millis() / 1000;
  String payload = String("{\"scoutId\":\"") + SCOUT_ID +
                   String("\",\"timestamp\":") + ts +
                   String(",\"modules\":[");
  for (size_t i = 0; i < moduleCount; ++i) {
    float v = modules[i].read();
    payload += String("{\"type\":\"") + modules[i].label +
               String("\",\"value\":") +
               (isnan(v) ? String("null") : String(v, 3)) +
               String("}");
    if (i + 1 < moduleCount) payload += ",";
  }
  payload += String("]}");

  // — Publish telemetry —
  Serial.print(F("📡 Publishing: "));
  Serial.println(payload);
  mqttClient.beginMessage(MQTT_TOPIC);
    mqttClient.print(payload);
  mqttClient.endMessage();

  // — Laser blink feedback —
  bool sound = digitalRead(SOUND_DIGITAL);
  digitalWrite(LASER_PIN, HIGH);
  delay(sound ? 1000 : 5000);
  digitalWrite(LASER_PIN, LOW);
  delay(sound ? 1000 : 5000);

  // — Alien‑smiley animation —
  Serial.println(F("👽 Alien is beaming you a smile..."));
  matrix.renderBitmap(smiley0, 8, 12);
  delay(200);
  Serial.println(F("😉 Blink!"));
  matrix.renderBitmap(smiley1, 8, 12);
  delay(200);
}