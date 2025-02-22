using CRSim.Core.Converters;
using System.Text.Json.Serialization;

namespace CRSim.Core.Models
{
    public class Settings
    {
        [JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan TimeOffset { get; set; } = TimeSpan.Zero;

        [JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan DepartureCheckInAdvanceDuration { get; set; } = TimeSpan.FromMinutes(20);

        [JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan PassingCheckInAdvanceDuration { get; set; } = TimeSpan.FromMinutes(10);

        /// <summary>
        /// 直到发车前多久停止显示
        /// </summary>
        [JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan StopDisplayUntilDepartureDuration { get; set; } = TimeSpan.FromMinutes(1);

        [JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan StopCheckInAdvanceDuration { get; set; } = TimeSpan.FromMinutes(2);

        public int MaxPages { get; set; } = 3;
        public int SwitchPageSeconds { get; set; } = 20;
    }
}
