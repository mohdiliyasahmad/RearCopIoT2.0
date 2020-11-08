
/************************* Adafruit.io Setup *********************************/
#define AIO_SERVER      "io.adafruit.com"
#define AIO_SERVERPORT  1883                   // use 8883 for SSL
#define AIO_USERNAME    "ilyasahmad"
#define AIO_KEY         "aio_iSQK18L67hQmEZyWNSekAScN6mUP"
/************ Global State (you don't need to change this!) ******************/


String deviceId = String(ESP.getChipId()) + String(ESP.getSketchMD5()) + String(ESP.getFlashChipId());

String D2C_DEVICEPING = "ilyasahmad/feeds/rcdeviceping";
String C2D_ENDPOINT = "ilyasahmad/feeds/"+  deviceId + ".c2d";
String D2C_ENDPOINT = "ilyasahmad/feeds/"+  deviceId + ".d2c";

char * string2char(String command){
    if(command.length()!=0){
        char *p = const_cast<char*>(command.c_str());
        return p;
    }
}


int dBmtoPercentage(int dBm)
{
 
    if(dBm <= RSSI_MIN)
    {
        quality = 0;
    }
    else if(dBm >= RSSI_MAX)
    {  
        quality = 100;
    }
    else
    {
        quality = 2 * (dBm + 100);
   }

     return quality;
}//dBmtoPercentage 


void initWifi()
{
 
  wifiManager.autoConnect(string2char("RC-4C"));
  //wifiManager.resetSettings();
 
  while (WiFi.status() != WL_CONNECTED) {
    Serial.print(".");
    delay(500);
  }

  if (WiFi.status() == WL_NO_SSID_AVAIL)
  {
      wifiManager.resetSettings();
  }
  else
  {

    Serial.println("Device:" + deviceId +"- connected with wifi:" + WiFi.SSID());
    Serial.println("IP address: "); 
    Serial.println(WiFi.localIP());
    Serial.print("WIFI Signal strength :");
    Serial.println(dBmtoPercentage(WiFi.RSSI()));
  }
 
 
  
}


void registerDevice()
{
 
  String url = "https://" + String(host) + "/gateway/"+ deviceId +"/RegisterDevice";

  Serial.println(deviceId);
  Serial.println(url);
  
  std::unique_ptr<BearSSL::WiFiClientSecure>clientHttp(new BearSSL::WiFiClientSecure);
  clientHttp->setInsecure();//(fingerprint);

  
  HTTPClient http; //Object of class HTTPClient
    http.begin(*clientHttp,url);
    http.addHeader("Ocp-Apim-Subscription-Key", aipmkey);
    int httpCode = http.GET();
    Serial.println(httpCode);
    if (httpCode == 200) 
    {
      Serial.println("Device registered successfully");
    }
    else
    {
       while (1);
    }
    http.end(); //Close connection
 
}


void getDeviceDefaults()
{
  String url = "https://" + String(host) + "/gateway/"+ deviceId +"/get";
  Serial.println(url);

  std::unique_ptr<BearSSL::WiFiClientSecure>clientHttp(new BearSSL::WiFiClientSecure);
  clientHttp->setInsecure();//(fingerprint);

  HTTPClient http; //Object of class HTTPClient
    http.begin(*clientHttp,url);
    http.addHeader("Ocp-Apim-Subscription-Key", aipmkey);
    int httpCode = http.GET();
    Serial.println(httpCode);
    if (httpCode == 200) 
    {
      deviceValue = http.getString();
      Serial.println("Device default values collected");
    }
    else
    {
      Serial.println("Device end point not found");
      while (1);
    }
  http.end(); //Close connection
 
}


void c2dcallback(char *data, uint16_t len) {
  Serial.print("c2d value is: ");
  Serial.println(data);
}


void DeviceReset()
{
    if (ESP.getHeapFragmentation() > 90)
    {
      ESP.restart();
    }  

  /*
    if (currentMillis - startMillis >= 86400000)
    {
       ESP.restart();
    }
  */

}



String detectHardware(char *payload)
{
  StaticJsonDocument<MESSAGE_MAX_LEN> root;
  deserializeJson(root, String(payload));
  String temp = root["defaultValue"];
  return temp;
}








/*
 
    pinMode(LED_BUILTIN, OUTPUT);
    digitalWrite(LED_BUILTIN, HIGH);
    Serial.begin(115200);
    //setupRelay();
    initWifi();
    Serial.println(string2char(String(USERNAME) +  deviceId + String(C2D)));
    
    //c2d.setCallback(c2dcallback);
    // Setup MQTT subscription for time feed.
    registerDevice();
    getDeviceDefaults();
    mqtt.subscribe(&c2d);
 
 */
