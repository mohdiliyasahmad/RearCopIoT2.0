#include <ArduinoJson.h>

void initSensor()
{
    // use SIMULATED_DATA, no sensor need to be inited
}

float readTemperature()
{
    return random(20, 30);
}

float readHumidity()
{
    return random(30, 40);
}

bool readMessage(char *payload)
{
    //float temperature = readTemperature();
    //float humidity = readHumidity();
    bool temperatureAlert = false;
    StaticJsonDocument<MESSAGE_MAX_LEN> root;
    
    //JsonObject &root = jsonBuffer.createObject();
    deserializeJson(root, payload);
    
    root["deviceId"] = deviceId;

    /*
    // NAN is not the valid json, change it to NULL
    if (std::isnan(temperature))
    {
        root["temperature"] = NULL;
    }
    else
    {
        root["temperature"] = temperature;
        if (temperature > TEMPERATURE_ALERT)
        {
            temperatureAlert = true;
        }
    }

    if (std::isnan(humidity))
    {
        root["humidity"] = NULL;
    }
    else
    {
        root["humidity"] = humidity;
    }
    */
    
    /* My modification */   
    //root["tempAlert"] = temperatureAlert;
    //root.printTo(payload, MESSAGE_MAX_LEN);
    //serializeJson(root, payload);
    return temperatureAlert;
}

void parseTwinMessage(char *message)
{
    //StaticJsonBuffer<MESSAGE_MAX_LEN> jsonBuffer;
    StaticJsonDocument<MESSAGE_MAX_LEN> root;
    
    //JsonObject &root = jsonBuffer.parseObject(message);
    auto error = deserializeJson(root, message);

    if (error) {
      Serial.printf("Parse %s failed.\r\n", message);
      return;
    }
    /*
    if (root["desired"]["interval"].success())
    {
        interval = root["desired"]["interval"];
    }
    else if (root.containsKey("interval"))
    {
        interval = root["interval"];
    }
    */
}
