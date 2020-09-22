using System;

namespace RearCop.Common
{
    public class HubDevice
    {
        public string DeviceId { get; set; }
        public string PrimeryKey { get; set; }
        public string DeviceConnectionString { get; set; }
        public string SaSToken { get; set; }
    }
}