using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace InvisualRest
{
  internal static class ObjectExtensions
  {
    internal static async Task<string> ToQueryStringAsync(this object instance, JsonSerializerSettings settings)
    {
      return await new FormUrlEncodedContent(
        JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(instance, settings), settings)
        )
        .ReadAsStringAsync();
    }
  }
}
