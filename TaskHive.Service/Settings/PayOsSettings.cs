using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskHive.Service.Settings
{
    public class PayOsSettings
    {
        public string ClientId { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ChecksumKey { get; set; } = string.Empty;
        public string? PartnerCode { get; set; }
    }
}
