// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Please use an Arduino IDE 1.6.8 or greater

#include <ESP8266WiFi.h>
#include <ESP8266HTTPClient.h>
#include <WiFiManager.h>

#include <AzureIoTHub.h>
#include <AzureIoTProtocol_MQTT.h>
#include <AzureIoTUtility.h>
#include <ArduinoJson.h>

#include "config.h"

String host = "172.20.10.4";
int port = 80;
String ApiKey ="295e3873-cc5b-479b-9274-6b99f2d80255";
static int messageCount = 1;
String deviceId = "";

unsigned long startMillis;  //some global variables available anywhere in the program
unsigned long currentMillis;

String connectionString;
static int interval = INTERVAL;
const int KEEP_ALIVE_TIMEOUT_S = 60;

WiFiManager wifiManager;

void initWifi()
{
  startMillis = millis();  //initial start time
  
  wifiManager.autoConnect("Svichex");  
  
  while (WiFi.status() != WL_CONNECTED) {
    Serial.print(".");
    delay(500);
  }

  if (WiFi.status() == WL_NO_SSID_AVAIL)
  {
      wifiManager.resetSettings();
  }

   Serial.println("Svichex connected with wifi:" + WiFi.SSID());
}

void initTime()
{
    time_t epochTime;
    configTime(0, 0, "pool.ntp.org", "time.nist.gov");

    while (true)
    {
        epochTime = time(NULL);

        if (epochTime == 0)
        {
            Serial.println("Fetching NTP epoch time failed! Waiting 2 seconds to retry.");
            delay(2000);
        }
        else
        {
            Serial.printf("Fetched NTP epoch time is: %lu.\r\n", epochTime);
            break;
        }
    }
}

char * string2char(String command){
    if(command.length()!=0){
        char *p = const_cast<char*>(command.c_str());
        return p;
    }
}

void initDeviceInfo()
{
  deviceId = WiFi.macAddress();
  deviceId.replace(":", String(ESP.getChipId()));
 
  String url = "http://" + String(host) + "/"+ deviceId +"/Gateway/FetchDevice?APIKey=" + ApiKey;

  Serial.println(deviceId);
  Serial.println(url);

  HTTPClient http; //Object of class HTTPClient
    http.begin(url);
    int httpCode = http.GET();
    Serial.println(httpCode);
    if (httpCode == 200) 
    {
      connectionString = http.getString();
      Serial.print(connectionString);
    }
  http.end(); //Close connection
 
}


void initDeviceDefaults()
{
  String url = "http://" + String(host) + "/"+ deviceId +"/Gateway/ServiceGet/V16/?value=1&APIKey=" + ApiKey;
  Serial.println(url);

  HTTPClient http; //Object of class HTTPClient
    http.begin(url);
    int httpCode = http.GET();
    Serial.println(httpCode);
    if (httpCode == 200) 
    {
      //connectionString = httpDefaults.getString();
    }
    else
    {
      Serial.print("Device end point not found");
      ESP.restart();
    }
  http.end(); //Close connection
 
}

static IOTHUB_CLIENT_LL_HANDLE iotHubClientHandle;

void initIoThubClient()
{

    iotHubClientHandle = IoTHubClient_LL_CreateFromConnectionString(string2char(connectionString), MQTT_Protocol);
    if (iotHubClientHandle == NULL)
    {
        Serial.println("Failed on IoTHubClient_CreateFromConnectionString.");
        while (1);
    }

    IoTHubClient_LL_SetOption(iotHubClientHandle, OPTION_KEEP_ALIVE, &KEEP_ALIVE_TIMEOUT_S);
    IoTHubClient_LL_SetMessageCallback(iotHubClientHandle, receiveMessageCallback, NULL);
    IoTHubClient_LL_SetDeviceMethodCallback(iotHubClientHandle, deviceMethodCallback, NULL);
    IoTHubClient_LL_SetDeviceTwinCallback(iotHubClientHandle, twinCallback, NULL);
}


void setup()
{
    pinMode(LED_BUILTIN, OUTPUT);
    digitalWrite(LED_BUILTIN, HIGH);
    initSerial();
    delay(1000);
    setupRelay();
    initWifi();
    initTime();
    initDeviceInfo();
    initIoThubClient();
    delay(1000);
    initDeviceDefaults();
}

void DeviceReset()
{
    if (ESP.getHeapFragmentation() > 90)
    {
      ESP.restart();
    }  
  
    if (currentMillis - startMillis >= 86400000)
    {
       ESP.restart();
    }
}

void loop()
{
    currentMillis = millis();
    DeviceReset();
    //Serial.println(currentMillis - startMillis);
    //char messagePayload[MESSAGE_MAX_LEN];
    //bool temperatureAlert = readMessage(messageCount, messagePayload);
    //sendMessage(iotHubClientHandle, messagePayload, temperatureAlert);
    delay(100);
    IoTHubClient_LL_DoWork(iotHubClientHandle);
}
