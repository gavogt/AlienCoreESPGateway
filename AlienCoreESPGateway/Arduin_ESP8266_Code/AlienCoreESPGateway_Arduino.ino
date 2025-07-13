#include <SoftwareSerial.h>
#include <WiFiEsp.h>
#include <PubSubClient.h>

SoftwareSerial EspSerial(4, 5);      // RX to D4, TX to D5
char ssid[]     = "Your SSID";
char pass[]     = "Your password";

const char* mqttServer = "192.168.x.xxx";  
const uint16_t mqttPort = 1883;
const char*  mqttTopic = "xeno/uno-scout/sound";
WiFiEspClient wifiClient;
PubSubClient  mqttClient(wifiClient);

const uint8_t LASER_PIN     = 2;  // Laser “S” to D2
const uint8_t SOUND_DIGITAL = 3;  // Sound “D0” to D3

int lastSound = LOW;

void setup() {
  Serial.begin(115200);
  while (!Serial);

  // 1) Laser & sound I/O
  pinMode(LASER_PIN, OUTPUT);
  digitalWrite(LASER_PIN, LOW);
  pinMode(SOUND_DIGITAL, INPUT);

  // 2) Initialize ESP-12S AT-firmware link
  EspSerial.begin(9600);
  WiFi.init(&EspSerial);

  // 3) Sanity check: shield present?
  if (WiFi.status() == WL_NO_SHIELD) {
    Serial.println("ERROR: ESP8266 not detected");
    while (true);
  }

  // 4) Connect to Wi-Fi (blocks until successful)
  Serial.print("Connecting to WiFi \"");
  Serial.print(ssid);
  Serial.print("\"");
  while (WiFi.begin(ssid, pass) != WL_CONNECTED) {
    Serial.print('.');
    delay(2000);
  }
  Serial.println("\n✓ WiFi connected");

  // 5) Print assigned IP (IPv4 or IPv6)
  Serial.print("IP Address: ");
  Serial.println(WiFi.localIP());

  // 6) Configure MQTT broker
  mqttClient.setServer(mqttServer, mqttPort);
}

void reconnectMQTT() {
  while (!mqttClient.connected()) {
    Serial.print("Connecting to MQTT…");
    if (mqttClient.connect("uno-scout")) {
      Serial.println("connected");
    } else {
      Serial.print("failed, rc=");
      Serial.print(mqttClient.state());
      Serial.println("; retry in 5s");
      delay(5000);
    }
  }
}

void loop() {
  // Maintain Wi-Fi
  if (WiFi.status() != WL_CONNECTED) {
    Serial.println("WiFi lost, reconnecting…");
    WiFi.begin(ssid, pass);
    delay(2000);
    return;
  }

  // Maintain MQTT
  if (!mqttClient.connected()) reconnectMQTT();
  mqttClient.loop();

  // read sound
  int s = digitalRead(SOUND_DIGITAL);
  unsigned long t = millis();

  // publish state
  bool ok = mqttClient.publish(mqttTopic, s ? "1" : "0");

  Serial.print(t);
  Serial.print(" ms Sound=");
  Serial.print(s);
  Serial.print(" → MQTT ");
  Serial.println(ok ? "OK" : "FAIL");

  // Blink laser
  if (s) {
    digitalWrite(LASER_PIN, HIGH); delay(100);
    digitalWrite(LASER_PIN, LOW);  delay(100);
  } else {
    digitalWrite(LASER_PIN, HIGH); delay(500);
    digitalWrite(LASER_PIN, LOW);  delay(500);
  }
}