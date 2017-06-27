using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace InvisualRestConverters
{
  public class JsonStringifier : JsonConverter
  {
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      var stringWriter = new StringWriter();
      var textWriter = new JsonTextWriter(stringWriter);
      serializer.Serialize(textWriter, value);

      serializer.Serialize(writer, stringWriter.ToString());
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      if (reader.TokenType == JsonToken.Null)
        return null;

      JObject json = JObject.Parse(reader.Value.ToString());

      return serializer.Deserialize(new JTokenReader(json), objectType);
    }

    public override bool CanConvert(Type objectType)
    {
      return true;
    }
  }
}
