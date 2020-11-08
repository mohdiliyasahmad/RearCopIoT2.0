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
#include <ArduinoJson.h>
#include "Adafruit_MQTT.h"
#include "Adafruit_MQTT_Client.h"

#include "config.h"
#include "Relays.h"
#include "sketch.h"

/************ Global State (you don't need to change this!) ******************/

// Create an ESP8266 WiFiClient class to connect to the MQTT server.
WiFiClient client;
// or... use WiFiClientSecure for SSL
//WiFiClientSecure client;

// Setup the MQTT client class by passing in the WiFi client and MQTT server and login details.
Adafruit_MQTT_Client mqtt(&client, AIO_SERVER, AIO_SERVERPORT, AIO_USERNAME, AIO_KEY);

/****************************** Feeds ***************************************/
/*
// Setup a feed called 'photocell' for publishing.
// Notice MQTT paths for AIO follow the form: <username>/feeds/<feedname>
Adafruit_MQTT_Publish d2c = Adafruit_MQTT_Publish(&mqtt, D2C_ENDPOINT.c_str());
*/


// Setup a feed called 'deviceping' for publishing.
Adafruit_MQTT_Publish d2cping = Adafruit_MQTT_Publish(&mqtt, AIO_USERNAME "/feeds/rcdeviceping");


// Setup a feed called 'onoff' for subscribing to changes.
Adafruit_MQTT_Subscribe c2d = Adafruit_MQTT_Subscribe(&mqtt, C2D_ENDPOINT.c_str());
/*************************** Sketch Code ************************************/

// Bug workaround for Arduino 1.6.6, it seems to need a function declaration
// for some reason (only affects ESP8266, likely an arduino-builder bug).
void MQTT_connect();

void setup() {
  
  initRelays();
  delay(100);
  Serial.begin(115200);
  
  initWifi();
  
  //isConnectedToWifi();
  //Registring device id
  registerDevice();
  // getting default values
  getDeviceDefaults();

  ControlRelay(string2char(deviceValue));
  
  // Setup MQTT subscription
  mqtt.subscribe(&c2d);

  startMillis = millis();
  deviceMillis = millis();
    
}


void reConnect()
{
    if (currentMillis - deviceMillis >= ((1000)* 60)*5)
    {
        deviceMillis =millis();
        mqtt.disconnect();
    }
}


void loop() {
  currentMillis =millis();

  // Ensure the connection to the MQTT server is alive (this will make the first
  // connection and automatically reconnect when disconnected).  See the MQTT_connect
  // function definition further below.
  MQTT_connect();

  // this is our 'wait for incoming subscription packets' busy subloop
  // try to spend your time here

  Adafruit_MQTT_Subscribe *subscription;
  while ((subscription = mqtt.readSubscription(5000))) {
    if (subscription == &c2d) {
      Serial.println((char *)c2d.lastread);
        //detecting hardware state
        isHardwareConnected = detectHardware((char *)c2d.lastread);
        blinkLED();
        if(isHardwareConnected == "IsHardwareConnected")
        {
          Serial.print(F("\nSending deviceping"));
          if (! d2cping.publish(String(deviceId +":IsHardwareConnected").c_str())) {
            Serial.println(F("Hardware callback Failed"));
          } else {
            Serial.println(F("Hardware callback success"));
          }
        }
        else
        {
          ControlRelay((char *)c2d.lastread);
          delay(INTERVAL);
        }

       
        
        /*
        // Now we can publish stuff!
        Serial.print(F("\nSending d2c"));
        Serial.print(x);
        Serial.print("...");
        if (! d2c.publish(x++)) {
          Serial.println(F("Failed"));
        } else {
          Serial.println(F("OK!"));
        }
        */
        
    }
 
  }

  // ping the server to keep the mqtt connection alive
  // NOT required if you are publishing once every KEEPALIVE seconds
  /*
  if(! mqtt.ping()) {
    mqtt.disconnect();
  }
  */
  
  DeviceReset();
  reConnect();
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

  uint8_t retries = 20000;
  while ((ret = mqtt.connect()) != 0) { // connect will return 0 for connected
       Serial.println(mqtt.connectErrorString(ret));
       Serial.println("Retrying MQTT connection in 5 seconds...");
       digitalWrite(Relay1, LOW);
       digitalWrite(Relay0, HIGH);
       mqtt.disconnect();
       delay(2000);  // wait 5 seconds
       retries--;
       if (retries == 0) {
         // basically die and wait for WDT to reset me
         while (1);
       }
  }
  Serial.println("MQTT Connected!");
  digitalWrite(Relay0, LOW);
  digitalWrite(Relay1, HIGH);
}
