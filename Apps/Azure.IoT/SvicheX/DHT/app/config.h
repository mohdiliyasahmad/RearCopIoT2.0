// Pin layout configuration
#define LED_PIN 0
#define DHTPIN 5
#define DHTTYPE DHT22
#define LED1 16 //D0
#define LED2 4 //D2
#define LED3 15 //D8


// Interval time(ms) for sending message to IoT Hub
#define INTERVAL 1000

// EEPROM address configuration
#define EEPROM_SIZE 512

#define MESSAGE_MAX_LEN 256

const int RSSI_MAX =-50;// define maximum strength of signal in dBm
const int RSSI_MIN =-100;// define minimum strength of signal in dBm
int quality;
 
String host = "rciot.azure-api.net";
String aipmkey ="3d6cc6a29c8c43549769a7ba23d6f6ca";


unsigned long startMillis;  //some global variables available anywhere in the program
unsigned long currentMillis;
unsigned long deviceMillis;

String isHardwareConnected;
String deviceValue;
String returnValue;
