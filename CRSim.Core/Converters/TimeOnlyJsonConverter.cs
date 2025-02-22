using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace CRSim.Core.Converters
{
    public class TimeOnlyJsonConverter : JsonConverter<DateTime?>
    {
        private const string TimeFormat = "HH:mm:ss";

        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string timeString = reader.GetString();
                if (DateTime.TryParseExact(timeString, TimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime result))
                {
                    return result;
                }
            }
            return null; // 处理 null 值
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
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
