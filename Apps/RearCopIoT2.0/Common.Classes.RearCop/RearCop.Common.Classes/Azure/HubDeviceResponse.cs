namespace RearCop.Common
{
    public class HubDeviceResponse : HubDevice
    {
        public string GenerationId { get; set; }
        public string Etag { get; set; }
        public string ConnectionState { get; set; }
        public string Status { get; set; }
        public string StatusReason { get; set; }
        public string LastActivityTime { get; set; }
        public int CloudToDeviceMessageCount { get; set; }
        public HubAuthentication Authentication { get; set; }
    }
}