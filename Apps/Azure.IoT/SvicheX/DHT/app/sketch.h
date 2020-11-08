
WiFiManager wifiManager;

DHT dht(DHTPIN, DHTTYPE); // 11 works fine for ESP8266
float humidity, temp_f;  // Values read from sensor 
unsigned long previousMillis = 0;        // will store last temp was read

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
  startMillis = millis();  //initial start time
  wifiManager.autoConnect(string2char(ESP.getChipId()));
 
  while (WiFi.status() != WL_CONNECTED) {
    Serial.print(".");
    delay(500);
  }

  if (WiFi.status() == WL_NO_SSID_AVAIL)
  {
      wifiManager.resetSettings();
  }

  Serial.println("Device:" + deviceId +"- connected with wifi:" + WiFi.SSID());
  Serial.println("IP address: "); 
  Serial.println(WiFi.localIP());
  Serial.print("WIFI Signal strength :");
  Serial.println(dBmtoPercentage(WiFi.RSSI()));
  
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
       ESP.restart();
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
      ESP.restart();
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
  
    if (currentMillis - startMillis >= 86400000)
    {
       ESP.restart();
    }

}




String detectHardware(char *payload)
{
  StaticJsonDocument<MESSAGE_MAX_LEN> root;
  deserializeJson(root, String(payload));
  String temp = root["defaultValue"];
  return temp;
}

void blinkLED()
{
    digitalWrite(LED_BUILTIN, LOW);
    digitalWrite(LED1, LOW);
    delay(100);
    digitalWrite(LED_BUILTIN, HIGH);
    digitalWrite(LED1, HIGH);
}

void gettemperature() {
  
    // Reading temperature for humidity takes about 250 milliseconds!
    // Sensor readings may also be up to 2 seconds 'old' (it's a very slow sensor)
    humidity = dht.readHumidity();      // Read humidity (percent)
    temp_f = dht.readTemperature();     // Read temperature as Fahrenheit
    
    // Check if any reads failed and exit early (to try again).
    if (isnan(humidity) || isnan(temp_f)) {
      Serial.println("Failed to read from DHT sensor!");
      digitalWrite(LED2, LOW);
      digitalWrite(LED1, LOW);
      return;
    }

    blinkLED();


   if(temp_f<=40)
    {
        digitalWrite(LED1, HIGH);
        digitalWrite(LED2, LOW);
    }

   if(temp_f>40)
    {
        digitalWrite(LED2, HIGH);
        digitalWrite(LED1, LOW);
    }
}
