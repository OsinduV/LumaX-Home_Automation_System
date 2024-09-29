#include <WiFi.h>
#include <PubSubClient.h>
#include <DHT.h>

// WiFi credentials
const char* ssid = "Dialog 4G 588";
const char* password = "afAdddd1";

// MQTT Broker details
const char* mqtt_server = "192.168.8.169"; // You can use your own broker address

// MQTT topics
const char* temp_topic = "home/room1/temperature/read";  // Publish temperature
const char* light_command_topic = "home/room1/light1/command";  // Subscribe to light commands
const char* light_status_topic = "home/room1/light1/status";  // Publish light status

// Pin Definitions
#define DHTPIN 4          // DHT sensor pin
#define LIGHTPIN 2        // Relay pin for light control
#define DHTTYPE DHT11     // DHT 11

// Define relay active type
bool relayActiveState = HIGH; // Set to LOW for active LOW relay, HIGH for active HIGH relay

// Initialize DHT sensor
DHT dht(DHTPIN, DHTTYPE);

// Initialize WiFi and MQTT clients
WiFiClient espClient;
PubSubClient client(espClient);

// Timing variables for non-blocking code
unsigned long lastDHTReadTime = 0;
const long dhtReadInterval = 5000; // 5-second interval for DHT readings

// Function to connect to WiFi
void setup_wifi() {
  delay(10);
  Serial.println();
  Serial.print("Connecting to ");
  Serial.println(ssid);

  WiFi.begin(ssid, password);

  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }

  Serial.println("");
  Serial.println("WiFi connected. ");
  Serial.print("IP address: ");
  Serial.println(WiFi.localIP());
}

// Function to reconnect to MQTT broker
void reconnect() {
  while (!client.connected()) {
    Serial.print("Attempting MQTT connection...");
    if (client.connect("ESP32Client")) {
      Serial.println("connected");
      // Subscribe to the light control topic
      client.subscribe(light_command_topic);
    } else {
      Serial.print("failed, rc=");
      Serial.print(client.state());
      Serial.println(" try again in 5 seconds");
      delay(5000);
    }
  }
}

// Function to handle incoming MQTT messages
void callback(char* topic, byte* payload, unsigned int length) {
  Serial.print("Message arrived on topic: ");
  Serial.println(topic);

  // Create a buffer to hold the incoming message
  char messageTemp[length + 1];
  memcpy(messageTemp, payload, length);
  messageTemp[length] = '\0';  // Null-terminate the string

  Serial.print("Payload: ");
  Serial.println(messageTemp);

  // If the message is on the light command topic
  if (strcmp(topic, light_command_topic) == 0) {
    if (strcmp(messageTemp, "ON") == 0) {
      digitalWrite(LIGHTPIN, relayActiveState);  // Turn light ON
      client.publish(light_status_topic, "ON", true);  // Retained message
      Serial.println("Light turned ON");
    } else if (strcmp(messageTemp, "OFF") == 0) {
      digitalWrite(LIGHTPIN, !relayActiveState);  // Turn light OFF
      client.publish(light_status_topic, "OFF", true);  // Retained message
      Serial.println("Light turned OFF");
    }
  }
}


void setup() {
  Serial.begin(115200);
  pinMode(LIGHTPIN, OUTPUT);
  digitalWrite(LIGHTPIN, !relayActiveState);  // Default state: light OFF

  // Start DHT sensor
  dht.begin();

  // Connect to WiFi
  setup_wifi();

  // Set MQTT server and callback function
  client.setServer(mqtt_server, 1883);
  client.setCallback(callback);
}

void loop() {
  // Ensure MQTT connection
  if (!client.connected()) {
    reconnect();
  }
  client.loop();
  
  unsigned long currentMillis = millis();
  if (currentMillis - lastDHTReadTime >= dhtReadInterval) {
    lastDHTReadTime = currentMillis;
    // Read and publish temperature
    float temp = dht.readTemperature();
    // Publish temperature data
    if (isnan(temp)) {
      Serial.println("Failed to read from DHT sensor!");
    }else{
      char tempStr[8];
      dtostrf(temp, 6, 2, tempStr); //converts the floating-point temperature into a string format (character array). 6: The minimum number of characters for the resulting string, 2 means it will round to two decimal places.
      client.publish(temp_topic, tempStr);
      Serial.print("Temperature: ");
      Serial.println(tempStr);
    }
  }

}

/*
* Wait a few seconds between measurements.
* Reading temperature or humidity takes about 250 milliseconds!
* Sensor readings may also be up to 2 seconds 'old' (its a very slow sensor)

* 
  float temp = dht.readTemperature(); // Read temperature as Celsius (the default)
  float f = dht.readTemperature(true); // Read temperature as Fahrenheit (isFahrenheit = true)
  float h = dht.readHumidity(); //Read humidity

*/
