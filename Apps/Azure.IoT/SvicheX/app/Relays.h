
//Relays
#define Relay0 16 //D0
#define Relay1 5 //D1
#define Relay2 4 //D2

#define Relay4 14 //D5
#define Relay5 12 //D6
#define Relay6 13 //D7
#define Relay7 15 //D8


void initRelays()
{
  //Onboard LED
  pinMode(LED_BUILTIN, OUTPUT);
  digitalWrite(LED_BUILTIN, HIGH);
 
  pinMode(Relay0, OUTPUT); 
  pinMode(Relay1, OUTPUT); 
  pinMode(Relay2, OUTPUT); 
  
  pinMode(Relay4, OUTPUT); 
  pinMode(Relay5, OUTPUT); 
  pinMode(Relay6, OUTPUT); 
  pinMode(Relay7, OUTPUT); 

 /*
  digitalWrite(Relay0, LOW);
  digitalWrite(Relay1, LOW);
  digitalWrite(Relay2, LOW);

  digitalWrite(Relay4, LOW);
  digitalWrite(Relay5, LOW);
  digitalWrite(Relay6, LOW);
  digitalWrite(Relay7, LOW);
*/

  digitalWrite(Relay0, LOW); 
  digitalWrite(Relay1, HIGH); 
   
  

}

void ControlRelay(char *payload)
{
  StaticJsonDocument<MESSAGE_MAX_LEN> root;
  deserializeJson(root, String(payload));

  Serial.println(String(payload));
  
    if(root["v1"]=="1")
    {
       digitalWrite(Relay4, HIGH);
    }
    
    if(root["v1"]=="0")
    {
       digitalWrite(Relay4, LOW);
    }

    if(root["v2"]=="1")
    {
       digitalWrite(Relay5, HIGH);
    }
    if(root["v2"]=="0")
    {
       digitalWrite(Relay5, LOW);
    }

    if(root["v3"]=="1")
    {
       digitalWrite(Relay6, HIGH);
    }
    if(root["v3"]=="0")
    {
       digitalWrite(Relay6, LOW);
    }

    if(root["v4"]=="1")
    {
       digitalWrite(Relay7, HIGH);
    }
    if(root["v4"]=="0")
    {
       digitalWrite(Relay7, LOW);
    }

    /*
    if(root["v5"]=="1")
    {
       digitalWrite(Relay5, HIGH);
    }
     if(root["v5"]=="0")
    {
       digitalWrite(Relay5, LOW);
    }

    if(root["v6"]=="1")
    {
       digitalWrite(Relay6, HIGH);
    }
    if(root["v6"]=="0")
    {
       digitalWrite(Relay6, LOW);
    }

    if(root["v7"]=="1")
    {
       digitalWrite(Relay7, HIGH);
    }
    if(root["v7"]=="0")
    {
       digitalWrite(Relay7, LOW);
    }
    */
    
}


void blinkLED()
{
    digitalWrite(LED_BUILTIN, LOW);
    digitalWrite(Relay1, LOW);
    delay(50);
    digitalWrite(LED_BUILTIN, HIGH);
    digitalWrite(Relay1, HIGH);
}

void isConnectedToWifi()
{
    if(WiFi.status() == WL_CONNECTED)
    {
       digitalWrite(Relay0, LOW);
       digitalWrite(Relay1, HIGH);
       
    }
    else
    {
      digitalWrite(Relay0, HIGH);
      digitalWrite(Relay1, LOW);
    }
    
}
