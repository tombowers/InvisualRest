﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;

namespace InvisualRest.JsonConverters
{
  /// <summary>
  /// JsonConverter for ISO dates.
  /// </summary>
  public class IsoDateOnlyDateTimeJsonConverter : DateTimeConverterBase
  {
    private const string Format = "yyyy-MM-dd";

    /// <summary>
    /// Overriden to read ISO dates.
    /// </summary>
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      if (reader.Value == null)
      {
        return null;
      }

      var s = reader.Value.ToString();
      DateTime result;
      if (DateTime.TryParseExact(s, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
      {
        return result;
      }

      return DateTime.Now;
    }

    /// <summary>
    /// Overriden to write ISO dates.
    /// </summary>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      writer.WriteValue(((DateTime)value).ToString(Format));
    }
  }
}
