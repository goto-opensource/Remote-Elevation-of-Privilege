using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace rEoP.Shared.Model
{
    //IMPROVE
    public class ConcurrentDictionaryConverter<T> : JsonConverter<ConcurrentDictionary<T, object>>
    {
        public override ConcurrentDictionary<T, object> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var arr = JsonSerializer.Deserialize<T[]>(ref reader, options);
            var ret = new ConcurrentDictionary<T, object>();
            foreach (var v in arr)
            {
                ret.TryAdd(v, new object());
            }

            return ret;
        }

        public override void Write(Utf8JsonWriter writer, ConcurrentDictionary<T, object> value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize<T[]>(writer, value.Keys.ToArray(), options);
        }
    }
}