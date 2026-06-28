using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;

namespace OvermorrowMod.Core.Particles.Modular
{
    /// <summary>
    /// Round-trips a <see cref="ParticleSpec"/> to/from JSON so tuned effects can be copied out of the
    /// editor and pasted back in as a baseline. Colors serialize as [r,g,b,a] and vectors as [x,y].
    /// </summary>
    public static class ParticleSpecSerializer
    {
        private static JsonSerializerSettings Settings() => new()
        {
            Formatting = Formatting.Indented,
            Converters = { new StringEnumConverter(), new ColorConverter(), new Vector2Converter() }
        };

        public static string Serialize(ParticleSpec spec) => JsonConvert.SerializeObject(spec, Settings());

        public static bool TryDeserialize(string json, out ParticleSpec spec)
        {
            spec = null;
            if (string.IsNullOrWhiteSpace(json)) return false;
            try
            {
                spec = JsonConvert.DeserializeObject<ParticleSpec>(json, Settings());
                return spec != null;
            }
            catch
            {
                return false;
            }
        }

        private class ColorConverter : JsonConverter<Color>
        {
            public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
            {
                writer.WriteStartArray();
                writer.WriteValue(value.R);
                writer.WriteValue(value.G);
                writer.WriteValue(value.B);
                writer.WriteValue(value.A);
                writer.WriteEndArray();
            }

            public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                JArray a = JArray.Load(reader);
                return new Color((int)a[0], (int)a[1], (int)a[2], a.Count > 3 ? (int)a[3] : 255);
            }
        }

        private class Vector2Converter : JsonConverter<Vector2>
        {
            public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
            {
                writer.WriteStartArray();
                writer.WriteValue(value.X);
                writer.WriteValue(value.Y);
                writer.WriteEndArray();
            }

            public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                JArray a = JArray.Load(reader);
                return new Vector2((float)a[0], (float)a[1]);
            }
        }
    }
}
