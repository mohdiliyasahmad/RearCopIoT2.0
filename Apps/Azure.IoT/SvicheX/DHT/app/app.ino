/***************************************************
  Adafruit MQTT Library ESP8266 Example

  Must use ESP8266 Arduino from:
    https://github.com/esp8266/Arduino

  Works great with Adafruit's Huzzah ESP board & Feather
  ----> https://www.adafruit.com/product/2471
  ----> https://www.adafruit.com/products/2821

  Adafruit invests time and resources providing this open source code,
  please support Adafruit and open-source hardware by purchasing
  products from Adafruit!

  Written by Tony DiCola for Adafruit Industries.
  MIT license, all text above must be included in any redistribution
 ****************************************************/

#include <ESP8266WiFi.h>
#include <ESP8266HTTPClient.h>
#include <WiFiManager.h>
#include <DHT.h>
#include <ArduinoJson.h>
#include "Adafruit_MQTT.h"
#include "Adafruit_MQTT_Client.h"
#include "config.h"
#include "sketch.h"

/************ Global State (you don't need to change this!) ******************/


// Create an ESP8266 WiFiClient class to connect to the MQTT server.
WiFiClient client;
// or... use WiFiClientSecure for SSL
//WiFiClientSecure client;

// Setup the MQTT client class by passing in the WiFi client and MQTT server and login details.
Adafruit_MQTT_Client mqtt(&client, AIO_SERVER, AIO_SERVERPORT, AIO_USERNAME, AIO_KEY);

/****************************** Feeds ***************************************/
// Setup a feed called 'photocell' for publishing.
// Notice MQTT paths for AIO follow the form: <username>/feeds/<feedname>
Adafruit_MQTT_Publish d2c = Adafruit_MQTT_Publish(&mqtt, D2C_ENDPOINT.c_str());

// Setup a feed called 'deviceping' for publishing.
Adafruit_MQTT_Publish d2cping = Adafruit_MQTT_Publish(&mqtt, AIO_USERNAME "/feeds/rcdeviceping");


// Setup a feed called 'onoff' for subscribing to changes.
Adafruit_MQTT_Subscribe c2d = Adafruit_MQTT_Subscribe(&mqtt, C2D_ENDPOINT.c_str());
/*************************** Sketch Code ************************************/

// Bug workaround for Arduino 1.6.6, it seems to need a function declaration
// for some reason (only affects ESP8266, likely an arduino-builder bug).
void MQTT_connect();

void readFromSubscription()
{
  Adafruit_MQTT_Subscribe *subscription;
  while ((subscription = mqtt.readSubscription(5000))) {
    if (subscription == &c2d) {
      Serial.print(F("Got: "));
      Serial.println((char *)c2d.lastread);
        
        //detecting hardware state
        isHardwareConnected = detectHardware((char *)c2d.lastread);
        
        if(isHardwareConnected == "IsHardwareConnected")
        {
          Serial.print(F("\nSending deviceping"));
          if (! d2cping.publish(String(deviceId +":IsHardwareConnected").c_str())) {
            Serial.println(F("Failed"));
          } else {
            Serial.println(F("OK!"));
          }
        }
    }
  }
}

void sendTemprature()
{
    if (currentMillis - deviceMillis >= INTERVAL)
    {
        deviceMillis =millis();
        gettemperature();
            // Now we can publish stuff!
       Serial.println("...Current temprature:");
       Serial.println(temp_f);
       Serial.println("...Current Humidity:");
       Serial.println(humidity);

       returnValue = "{\"temprature\":" + String(temp_f) +",\"humidity\":" + String(humidity)+ "}";

        if (!d2c.publish(returnValue.c_str())) {
          Serial.println(F("D2C Failed"));
        } else {
          Serial.println(F("D2C Success!"));
        }
    }
}

void setup  () {  
  pinMode(LED_BUILTIN, OUTPUT);
  digitalWrite(LED_BUILTIN, HIGH);
  pinMode(LED1, OUTPUT);
  pinMode(LED2, OUTPUT);
  pinMode(LED3, OUTPUT);
  
  digitalWrite(LED1, LOW);
  digitalWrite(LED2, LOW);
  digitalWrite(LED3, LOW);
     
  dht.begin();
  Serial.begin(115200);
  delay(100);
  
  digitalWrite(LED3, HIGH);
  initWifi();
  delay(50);
  digitalWrite(LED3, LOW);
  
  //Registring device id
  digitalWrite(LED3, HIGH);
  registerDevice();
  digitalWrite(LED3, LOW);
  delay(50);
 
  // getting default values
  // getDeviceDefaults();

  // Setup MQTT subscription
  mqtt.subscribe(&c2d);

  startMillis = millis();
  deviceMillis = millis();
  
}

void loop() {
  currentMillis = millis();
  // Ensure the connection to the MQTT server is alive (this will make the first
  // connection and automatically reconnect when disconnected).  See the MQTT_connect
  // function definition further below.
  MQTT_connect();
  // this is our 'wait for incoming subscription packets' busy subloop
  // try to spend your time here
  readFromSubscription();

  sendTemprature();

  DeviceReset();
}



// Function to connect and reconnect as necessary to the MQTT server.
// Should be called in the loop function and it will take care if connecting.
void MQTT_connect() {
  int8_t ret;

  // Stop if already connected.
  if (mqtt.connected()) {
    return;
  }
  Serial.print("Connecting to MQTT... ");
  
  uint8_t retries = 300000; // retry connecting for 5 min
  
  while ((ret = mqtt.connect()) != 0) { // connect will return 0 for connected
       Serial.println(mqtt.connectErrorString(ret));
       Serial.println("Retrying MQTT connection in 5 seconds...");
       digitalWrite(LED3, LOW);
       mqtt.disconnect();
       delay(INTERVAL);  // wait 1 seconds
       retries--;
       if (retries == 0) {
         // basically die and wait for WDT to reset me
         while (1);
       }
  }
  Serial.println("MQTT Connected!");
  digitalWrite(LED3, HIGH);
  
}
