using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace InvisualRest.JsonConverters
{
  /// <summary>
  /// JsonConverter to allow properties to contain JSON.
  /// This should only be used as a property-level annotation.
  /// </summary>
  public class JsonStringifier : JsonConverter
  {
    /// <summary>
    /// Overriden to write properly escaped JSON within a JSON field.
    /// </summary>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      var stringWriter = new StringWriter();
      var textWriter = new JsonTextWriter(stringWriter);
      serializer.Serialize(textWriter, value);

      serializer.Serialize(writer, stringWriter.ToString());
    }

    /// <summary>
    /// Overridden to read escaped JSON from within a JSON field.
    /// </summary>
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      if (reader.TokenType == JsonToken.Null)
        return null;

      JObject json = JObject.Parse(reader.Value.ToString());

      return serializer.Deserialize(new JTokenReader(json), objectType);
    }

    /// <summary>
    /// Overriden to always return true.
    /// </summary>
    public override bool CanConvert(Type objectType)
    {
      return true;
    }
  }
}
