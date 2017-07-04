using InvisualRest.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InvisualRest
{
  /// <summary>
  /// Client for connecting to resources over HTTP.
  /// </summary>
  public class JsonRestClient
  {
    private readonly Uri _baseUri;
    private readonly JsonRestClientOptions _options;

    /// <summary>
    /// Creates an instance of <see cref="JsonRestClient"/>.
    /// </summary>
    /// <param name="baseUri">The root uri of the API.</param>
    /// <param name="options">Optional configuration options for the client.</param>
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

    /// <summary>
    /// Gets the request headers. Use this collection for custom control of the headers prior to requests.
    /// </summary>
    public Dictionary<string, string> RequestHeaders { get; }

    /// <summary>
    /// Performs a GET request.
    /// </summary>
    /// <typeparam name="T">The resource type.</typeparam>
    /// <param name="resource">The relative resource path.</param>
    /// <returns>An instance of T, deserialised from the API response.</returns>
    /// <exception cref="ArgumentNullException"></exception><exception cref="RestException"></exception><exception cref="HttpRequestException"></exception>
    public async Task<T> GetAsync<T>(string resource)
    {
      if (resource == null) throw new ArgumentNullException(nameof(resource));

      return await RequestAsync<T>(resource, HttpMethod.Get).ConfigureAwait(false);
    }

    /// <summary>
    /// Performs a GET request.
    /// </summary>
    /// <typeparam name="T">The resource type.</typeparam>
    /// <param name="resource">The relative resource path.</param>
    /// <param name="request">An object representing the request. This will be searialised and added to the querystring.</param>
    /// <returns>An instance of T, deserialised from the API response.</returns>
    /// <exception cref="ArgumentNullException"></exception><exception cref="RestException"></exception><exception cref="HttpRequestException"></exception>
    public async Task<T> GetAsync<T>(string resource, object request)
    {
      if (resource == null) throw new ArgumentNullException(nameof(resource));

      var querystring = await request.ToQueryStringAsync(GetJsonSerializerSettings());

      if (!string.IsNullOrWhiteSpace(querystring))
        resource += "?" + querystring;

      return await RequestAsync<T>(resource, HttpMethod.Get).ConfigureAwait(false);
    }

    /// <summary>
    /// Performs a POST request.
    /// </summary>
    /// <typeparam name="T">The resource type.</typeparam>
    /// <param name="resource">The relative resource path.</param>
    /// <param name="request">An object representing the request.</param>
    /// <returns>An instance of T, deserialised from the API response.</returns>
    /// <exception cref="ArgumentNullException"></exception><exception cref="RestException"></exception><exception cref="HttpRequestException"></exception>
    public async Task<T> PostAsync<T>(string resource, object request)
    {
      if (resource == null) throw new ArgumentNullException(nameof(resource));

      return await RequestAsync<T>(resource, HttpMethod.Post, request).ConfigureAwait(false);
    }

    /// <summary>
    /// Performs a PATCH request.
    /// </summary>
    /// <typeparam name="T">The resource type.</typeparam>
    /// <param name="resource">The relative resource path.</param>
    /// <param name="request">An object representing the request.</param>
    /// <returns>An instance of T, deserialised from the API response.</returns>
    /// <exception cref="ArgumentNullException"></exception><exception cref="RestException"></exception><exception cref="HttpRequestException"></exception>
    public async Task<T> PatchAsync<T>(string resource, object request)
    {
      if (resource == null) throw new ArgumentNullException(nameof(resource));

      return await RequestAsync<T>(resource, new HttpMethod("PATCH"), request).ConfigureAwait(false);
    }

    /// <summary>
    /// Performs a PUT request.
    /// </summary>
    /// <typeparam name="T">The resource type.</typeparam>
    /// <param name="resource">The relative resource path.</param>
    /// <param name="request">An object representing the request.</param>
    /// <returns>An instance of T, deserialised from the API response.</returns>
    /// <exception cref="ArgumentNullException"></exception><exception cref="RestException"></exception><exception cref="HttpRequestException"></exception>
    public async Task<T> PutAsync<T>(string resource, object request)
    {
      if (resource == null) throw new ArgumentNullException(nameof(resource));

      return await RequestAsync<T>(resource, HttpMethod.Put, request).ConfigureAwait(false);
    }

    /// <summary>
    /// Performs a DELETE request.
    /// </summary>
    /// <typeparam name="T">The resource type.</typeparam>
    /// <param name="resource">The relative resource path.</param>
    /// <returns>An instance of T, deserialised from the API response.</returns>
    /// <exception cref="ArgumentNullException"></exception><exception cref="RestException"></exception><exception cref="HttpRequestException"></exception>
    public async Task<T> DeleteAsync<T>(string resource)
    {
      if (resource == null) throw new ArgumentNullException(nameof(resource));

      return await RequestAsync<T>(resource, HttpMethod.Delete).ConfigureAwait(false);
    }

    /// <summary>
    /// Performs an HTTP request.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="resource">The resource type.</param>
    /// <param name="httpMethod">The request method.</param>
    /// <param name="request">An object representing the request.</param>
    /// <returns>An instance of T, deserialised from the API response.</returns>
    /// <exception cref="ArgumentNullException"></exception><exception cref="RestException"></exception><exception cref="HttpRequestException"></exception>
    protected virtual async Task<T> RequestAsync<T>(string resource, HttpMethod httpMethod, object request = null)
    {
      return await RequestAfterDelayAsync<T>(0, resource, httpMethod, request).ConfigureAwait(false);
    }

    private async Task<T> RequestAfterDelayAsync<T>(int requestIndex, string resource, HttpMethod httpMethod, object request = null)
    {
      await Task.Delay(CalculateRequestDelay(requestIndex)).ConfigureAwait(false);

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

          if (_options.NonHttpSuccessCodeHandling == NonHttpSuccessCodes.ThrowException && !response.IsSuccessStatusCode)
          {
            throw new RestException("Non-success status code. Check the HttpStatusCode property.")
            {
              HttpStatusCode = (int)response.StatusCode,
              RawResponse = await response.Content.ReadAsStringAsync()
            };
          }
        }
        catch (Exception)
        {
          if (_options.RetryPolicy != null && _options.RetryPolicy.OnException && requestIndex < _options.RetryPolicy.MaxRetries)
          {
            return await RequestAfterDelayAsync<T>(requestIndex + 1, resource, httpMethod, request).ConfigureAwait(false);
          }

          throw;
        }

        if (
          _options.RetryPolicy != null
          && _options.RetryPolicy.HttpStatuses.Any()
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

    private int CalculateRequestDelay(int numPreviousAttempts)
    {
      if (_options.RetryPolicy == null)
        return 0;

      switch (_options.RetryPolicy.DelayInterval)
      {
        case RetryPolicy.RetryDelayInterval.SetInterval:
          return numPreviousAttempts == 0 ? 0 : _options.RetryPolicy.SetDelayMilliseconds;

        case RetryPolicy.RetryDelayInterval.ExponentialBackoff:
          return ((1 << numPreviousAttempts) - 1) / 2 * 1000;

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
