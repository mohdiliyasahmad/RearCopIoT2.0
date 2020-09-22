using System.Collections.Generic;

namespace RearCop.Common
{
    public class AppConfig
    {
        public string DeviceKey { get; set; }
        public string APIRoutePath { get; set; }
        public string ServerURL { get; set; }
        public string DatabaseServerUri { get; set; }
        public string DatabaseServerUriPostFix { get; set; }
        public string ServiceAPIKey { get; set; }
        public int OnlineInterval { get; set; }
        public string ResetTime { get; set; }
        public string IoTHubName { get; set; }
        public string IoTHubAPIVersion { get; set; }
        public List<IoTHubConfiguration>  IoTHubConnectionStrings { get; set; }
        public string ServiceKey { get; set; }
        public string MqttURL { get; set; }
        public string MqttUserName { get; set; }
        public string MqttKey { get; set; }

    }
}
