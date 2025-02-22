using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace CRSim.Core.Converters
{
    public class TimeSpanConverter : JsonConverter<TimeSpan>
    {
        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.TotalMinutes); // 以分钟为单位进行序列化
        }

        // 反序列化：将数值转换回 TimeSpan 类型
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // 这里假设 JSON 存储的是分钟的数值
            double minutes = reader.GetDouble();
            return TimeSpan.FromMinutes(minutes);
        }
    }
}
