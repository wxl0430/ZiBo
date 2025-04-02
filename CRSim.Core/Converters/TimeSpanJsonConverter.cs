using System.Text.Json.Serialization;
using System.Text.Json;

namespace CRSim.Core.Converters
{
    public class TimeSpanJsonConverter : JsonConverter<TimeSpan?>
    {
        private const string TimeFormat = @"hh\:mm\:ss";


        public override TimeSpan? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string timeString = reader.GetString();
                if (TimeSpan.TryParseExact(timeString, TimeFormat, null, System.Globalization.TimeSpanStyles.None, out TimeSpan result))
                {
                    return result;
                }
            }
            return null; // 处理 null 值
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteStringValue(value.Value.ToString(TimeFormat));
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }

}
