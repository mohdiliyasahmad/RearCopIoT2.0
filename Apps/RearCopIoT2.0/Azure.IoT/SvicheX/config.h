// Pin layout configuration
#define LED_PIN 0
#define DHT_PIN D1

#define TEMPERATURE_ALERT 26

// Interval time(ms) for sending message to IoT Hub
#define INTERVAL 1000

// If don't have a physical DHT sensor, can send simulated data to IoT hub
#define SIMULATED_DATA true

// EEPROM address configuration
#define EEPROM_SIZE 512

// SSID and SSID password's length should < 32 bytes
// http://serverfault.com/a/45509
#define SSID_LEN 32
#define PASS_LEN 32
#define CONNECTION_STRING_LEN 1024

#define MESSAGE_MAX_LEN 256
