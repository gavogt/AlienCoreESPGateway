using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace AlienCoreESPGateway
{
    internal class TelemetryMessage
    {
        [JsonPropertyName("scoutId")]
        public string ScoutId { get; set; } = default!;
        [JsonPropertyName("timestamp")]
        public long TimestampUnix { get; set; }

        [JsonIgnore]
        public DateTime Timestamp => DateTimeOffset.FromUnixTimeMilliseconds(TimestampUnix).LocalDateTime;
        
        [JsonPropertyName("modules")]

        public List<ModuleReading> Modules { get; set; } = new List<ModuleReading>();
    }
}
