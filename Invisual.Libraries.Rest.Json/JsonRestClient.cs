using InvisualRest.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InvisualRest
{
  public class JsonRestClient
  {
    private readonly Uri _baseUri;
    private readonly JsonRestClientOptions _options;

    public JsonRestClient(string baseUri, JsonRestClientOptions options = null)
    {
      if (string.IsNullOrWhiteSpace(baseUri)) throw new ArgumentNullException(nameof(baseUri));

      _options = options ?? new JsonRestClientOptions();

      // Ensure the outermost directory is not discarded by the HttpClient by adding a slash
      if (!baseUri.EndsWith("/"))
        baseUri = baseUri + "/";

      _baseUri = new Uri(baseUri);

      RequestHeaders = new Dictionary<string, string>();

      if (_options.AuthenticationInfo != null)
        RequestHeaders.Add("Authorization", options.AuthenticationInfo.ToString());
    }

    public Dictionary<string, string> RequestHeaders { get; }

    public async Task<T> GetAsync<T>(string resource)
    {
      return await RequestAsync<T>(resource, HttpMethod.Get).ConfigureAwait(false);
    }

    public async Task<T> PostAsync<T>(string resource, object request)
    {
      return await RequestAsync<T>(resource, HttpMethod.Post, request).ConfigureAwait(false);
    }

    public async Task<T> PatchAsync<T>(string resource, object request)
    {
      return await RequestAsync<T>(resource, new HttpMethod("PATCH"), request).ConfigureAwait(false);
    }

    public async Task<T> PutAsync<T>(string resource, object request)
    {
      return await RequestAsync<T>(resource, HttpMethod.Put, request).ConfigureAwait(false);
    }

    public async Task<T> DeleteAsync<T>(string resource)
    {
      return await RequestAsync<T>(resource, HttpMethod.Delete).ConfigureAwait(false);
    }

    protected virtual async Task<T> RequestAsync<T>(string resource, HttpMethod httpMethod, object request = null)
    {
      return await RequestAfterDelayAsync<T>(0, resource, httpMethod, request).ConfigureAwait(false);
    }

    private async Task<T> RequestAfterDelayAsync<T>(int requestIndex, string resource, HttpMethod httpMethod, object request = null)
    {
      await Task.Delay(CalculateNextRetryDelay(requestIndex)).ConfigureAwait(false);

      // Ensure resource doesn't start with a slash, which will make the HttpClient perform the request relative the the uri root rather than the full base address.
      if (resource.StartsWith("/"))
        resource = resource.Substring(1);

      using (var client = new EnhancedHttpClient())
      {
        client.BaseAddress = _baseUri;

        foreach (KeyValuePair<string, string> header in RequestHeaders)
          client.DefaultRequestHeaders.Add(header.Key, header.Value);

        HttpResponseMessage response;

        HttpContent data = request != null
          ? new StringContent(JsonConvert.SerializeObject(request, GetJsonSerializerSettings()), Encoding.UTF8, "application/json")
          : null;

        try
        {
          switch (httpMethod.Method)
          {
            case "GET":
              response = await client.GetAsync(resource).ConfigureAwait(false);
              break;

            case "POST":
              response = await client.PostAsync(resource, data).ConfigureAwait(false);
              break;

            case "PATCH":
              response = await client.PatchAsync(resource, data).ConfigureAwait(false);
              break;

            case "PUT":
              response = await client.PutAsync(resource, data).ConfigureAwait(false);
              break;

            case "DELETE":
              response = await client.DeleteAsync(resource).ConfigureAwait(false);
              break;

            default:
              throw new RestException("Unsupported HTTP Method specified");
          }

          if (_options.NonHttpSuccessCodeHandling == NonHttpSuccessCodes.ThrowException)
            response.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
          if (
            (_options.RetryPolicy.WhenToRetry & RetryPolicy.Retry.OnAllExceptions) != 0
            && requestIndex < _options.RetryPolicy.MaxRetries
            )
          {
            return await RequestAfterDelayAsync<T>(requestIndex + 1, resource, httpMethod, request).ConfigureAwait(false);
          }

          throw new RestException("Exception thrown by HttpClient. See inner exception for details.", e);
        }

        if (
          (_options.RetryPolicy.WhenToRetry & RetryPolicy.Retry.OnSpecificResponseStatuses) != 0
          && requestIndex < _options.RetryPolicy.MaxRetries
          && _options.RetryPolicy.HttpStatuses.Contains((int)response.StatusCode)
          )
        {
          return await RequestAfterDelayAsync<T>(requestIndex + 1, resource, httpMethod, request).ConfigureAwait(false);
        }

        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        try
        {
          return JsonConvert.DeserializeObject<T>(content);
        }
        catch (Exception e)
        {
          throw new RestException("Exception thrown during deserialization. See inner exception.", e);
        }
      }
    }

    private int CalculateNextRetryDelay(int numPreviousAttempts)
    {
      switch (_options.RetryPolicy.DelayInterval)
      {
        case RetryPolicy.RetryDelayInterval.SetInterval:
          return numPreviousAttempts == 0 ? 0 : _options.RetryPolicy.SetDelayMilliseconds;

        case RetryPolicy.RetryDelayInterval.ExponentialBackoff:
          return ((1 << numPreviousAttempts) - 1) / 2 * 1000;

        case RetryPolicy.RetryDelayInterval.None:
        default:
          return 0;
      }
    }

    private JsonSerializerSettings GetJsonSerializerSettings()
    {
      var settings = new JsonSerializerSettings
      {
        NullValueHandling = _options.NullValueHandling == NullValues.Ignore ? NullValueHandling.Ignore : NullValueHandling.Include
      };

      if (_options.PropertyStyle == PropertyStyle.CamelCase)
        settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

      return settings;
    }

    private class EnhancedHttpClient : HttpClient
    {
      public async Task<HttpResponseMessage> PatchAsync(string requestUri, HttpContent content)
      {
        return await SendAsync(new HttpRequestMessage(new HttpMethod("PATCH"), requestUri) { Content = content }).ConfigureAwait(false);
      }
    }
  }
}
