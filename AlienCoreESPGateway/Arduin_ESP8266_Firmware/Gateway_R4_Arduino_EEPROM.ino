 
<!--
#include <WiFiS3.h>
#include <ArduinoMqttClient.h>
#include <Arduino_LED_Matrix.h>
#include <EEPROM.h>

// ─── I/O PINS ────────────────────────────────────────────────────────────────
const uint8_t LASER_PIN     = 2;   // D2 → laser
const uint8_t SOUND_DIGITAL = 3;   // D3 → sound‑sensor “D0”

// ─── EEPROM CONFIG STRUCT ────────────────────────────────────────────────────
struct Config { char ssid[32]; char pass[32]; char broker[16]; } cfg;

// ─── EEPROM HELPERS ──────────────────────────────────────────────────────────
bool freshEEPROM() {                        // checks if all bytes are 0xFF
for (size_t i = 0; i < sizeof(cfg); ++i) if (EEPROM.read(i) != 0xFF) return false;
return true;
}
void saveCfg() {
EEPROM.begin(); EEPROM.put(0,cfg);
Serial.println(F("💾 EEPROM → config saved"));
}
void loadCfg() {
EEPROM.begin();
if (freshEEPROM()) {
Serial.println(F("⚙️  EEPROM blank – seeding defaults"));
strncpy(cfg.ssid,"Your SSID",sizeof(cfg.ssid));
strncpy(cfg.pass,"Your Pass",sizeof(cfg.pass));
strncpy(cfg.broker,"192.168.x.xxx",sizeof(cfg.broker));
saveCfg();
} else {
EEPROM.get(0,cfg);
Serial.println(F("✅ EEPROM → config loaded"));
}
Serial.print  (F("   SSID   : ")); Serial.println(cfg.ssid);
Serial.print  (F("   PASS   : ")); Serial.println(cfg.pass);
Serial.print  (F("   BROKER : ")); Serial.println(cfg.broker);
}

// ─── CONFIG MENU ─────────────────────────────────────────────────────────────
void menu() {
Serial.println(F("\n=== SETUP MENU ==="));
Serial.println(F("1‑SSID  2‑PASS  3‑BROKER  S‑save  Q‑quit"));
while (true) {
while (!Serial.available());
char c = Serial.read(); Serial.read();               // drop CR/LF
if (c=='1'||c=='2'||c=='3') {
const char* lbl = (c=='1')?"SSID":(c=='2')?"PASS":"BROKER";
Serial.print(lbl); Serial.print(F(" > "));
char* dst = (c=='1')?cfg.ssid:(c=='2')?cfg.pass:cfg.broker;
size_t ln = (c=='1')?sizeof(cfg.ssid):(c=='2')?sizeof(cfg.pass):sizeof(cfg.broker);
while (!Serial.available());
Serial.readBytes(dst,ln-1); dst[ln-1]=0;
Serial.println(F("  ✓ updated"));
} else if (c=='S'||c=='s') {
saveCfg();
} else if (c=='Q'||c=='q') break;
}
Serial.println(F("Leaving menu → reboot or continue"));
}

// ─── SENSOR STUBS ────────────────────────────────────────────────────────────
float readNeuro()  { return analogRead(A0)*3.3/1023; }
float readPlasma() { return analogRead(A1)*3.3/1023; }
float readBio()    { return analogRead(A2)*3.3/1023; }
struct {const char* t; float(*f)();} sens[3] = {
{"NEURO",readNeuro},{"PLASMA",readPlasma},{"BIO",readBio}
};

// ─── LED‑MATRIX FRAMES ───────────────────────────────────────────────────────
uint8_t sm0[8][12]={{0,0,0,0,0,0,0,0,0,0,0,0},{0,1,1,1,1,1,1,1,1,1,1,0},
{1,0,0,0,0,0,0,0,0,0,0,1},{1,0,0,0,1,0,0,1,0,0,0,1},{1,0,0,0,1,1,1,1,0,0,0,1},
{1,0,0,0,0,0,0,0,0,0,0,1},{0,1,1,1,1,1,1,1,1,1,1,0},{0,0,0,0,0,0,0,0,0,0,0,0}};
uint8_t sm1[8][12]={{0,0,0,0,0,0,0,0,0,0,0,0},{0,1,1,1,1,1,1,1,1,1,1,0},
{1,0,0,0,0,0,0,0,0,0,0,1},{1,0,0,0,0,0,0,0,0,0,0,1},{1,0,0,0,1,1,1,1,0,0,0,1},
{1,0,0,0,0,0,0,0,0,0,0,1},{0,1,1,1,1,1,1,1,1,1,1,0},{0,0,0,0,0,0,0,0,0,0,0,0}};

// ─── GLOBAL OBJECTS ──────────────────────────────────────────────────────────
WiFiClient net;         // TCP socket
MqttClient mqtt(net);   // MQTT over Wi‑Fi
ArduinoLEDMatrix matrix;

// ─── CONNECT HELPERS ─────────────────────────────────────────────────────────
void connectWiFi(){
Serial.print(F("🔌 Wi‑Fi: connecting to \"")); Serial.print(cfg.ssid); Serial.println(F("\""));
WiFi.begin(cfg.ssid,cfg.pass);
while(WiFi.status()!=WL_CONNECTED){delay(500);Serial.print('.');}
Serial.print(F("\n   ↳ IP ")); Serial.println(WiFi.localIP());
}
void connectMQTT(){
mqtt.setId("xc‑scout‑01");
Serial.print(F("🔗 MQTT: broker ")); Serial.print(cfg.broker); Serial.println(F(":1883"));
while(!mqtt.connect(cfg.broker,1883)){delay(500);Serial.print('.');}
Serial.println(F("\n   ↳ session established"));
}

// ─── SETUP ───────────────────────────────────────────────────────────────────
void setup(){
Serial.begin(115200); while(!Serial);
Serial.println(F("\n🤖 UNO‑R4 Scout boot"));
loadCfg();
Serial.println(F("Press C in 5 s for menu…"));
unsigned long t0=millis(); bool menuWanted=false;
while(millis()-t0<5000){ if(Serial.available=""()&&Serial.read()=='C'){menu(); menuWanted=true;}}
if(menuWanted){ Serial.println(F("⚠️  Continuing with (possibly) new settings")); }

connectWiFi();
connectMQTT();

matrix.begin();
pinMode(LASER_PIN,OUTPUT);
pinMode(SOUND_DIGITAL,INPUT);
Serial.println(F("🚀 Setup complete — entering main loop"));
}

// ─── MAIN LOOP ───────────────────────────────────────────────────────────────
void loop(){
if(!mqtt.connected()){ Serial.println(F("⚠️  MQTT dropped — reconnecting")); connectMQTT();}
mqtt.poll();

// 1) Read sensors
float v0=sens[0].f(), v1=sens[1].f(), v2=sens[2].f();
Serial.print(F("📊 SENSE  → N=")); Serial.print(v0,3);
Serial.print(F("  P="));           Serial.print(v1,3);
Serial.print(F("  B="));           Serial.println(v2,3);

// 2) Build JSON
String js="{\"scoutId\":\"xc-scout-01\",\"timestamp\":"; js+=millis()/1000;
js+=",\"modules\":[";
js+="{\"type\":\"NEURO\",\"value\":"+String(v0,3)+"},";
js+="{\"type\":\"PLASMA\",\"value\":"+String(v1,3)+"},";
js+="{\"type\":\"BIO\",\"value\":"+String(v2,3)+"}]}";

// 3) Publish
Serial.print(F("📡 MQTT  → "));
mqtt.beginMessage("xeno/xc-scout-01/telemetry");
mqtt.print(js); mqtt.endMessage();
Serial.println(js);

// 4) Blink laser
bool loud=digitalRead(SOUND_DIGITAL);
Serial.print(F("🔦 Laser blink (")); Serial.print(loud?F("LOUD"):F("quiet")); Serial.println(F(")"));
digitalWrite(LASER_PIN,HIGH); delay(loud?1000:5000);
digitalWrite(LASER_PIN,LOW);  delay(loud?1000:5000);

// 5) LED‑matrix animation
Serial.println(F("👾 LED‑Matrix: smile frame 0"));
matrix.renderBitmap(sm0,8,12); delay(150);
Serial.println(F("😉 LED‑Matrix: smile frame 1"));
matrix.renderBitmap(sm1,8,12); delay(150);
}
