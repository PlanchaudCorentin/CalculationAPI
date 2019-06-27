using System;

namespace API.Models
{
    public class Stat
    {
        public long id { get; set; }
        public string mac_address { get; set; }
        public string device_type { get; set; }
        public decimal calculated_value { get; set; }
        public DateTime timestamp { get; set; }
    }
}