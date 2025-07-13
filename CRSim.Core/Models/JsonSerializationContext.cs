using System.Text.Json.Serialization;

namespace CRSim.Core.Models
{
    [JsonSerializable(typeof(Json))]
    [JsonSourceGenerationOptions(WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    public partial class JsonContext : JsonSerializerContext
    {
    }
}
