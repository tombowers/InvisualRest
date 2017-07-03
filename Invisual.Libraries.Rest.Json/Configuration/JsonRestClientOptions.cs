namespace InvisualRest.Configuration
{
  /// <summary>
  /// Provides configuration options for the JsonRestClient.
  /// </summary>
  public class JsonRestClientOptions
  {
    /// <summary>
    /// Details for Basic HTTP Authentication.
    /// </summary>
    public AuthenticationInfo AuthenticationInfo { get;set; }

    /// <summary>
    /// Upon receiving a non-200 status code, should an exception be thrown.
    /// </summary>
    public NonHttpSuccessCodes NonHttpSuccessCodeHandling { get; set; } = NonHttpSuccessCodes.Continue;

    /// <summary>
    /// Set whether or not null values should be included when serialising request data.
    /// </summary>
    public NullValues NullValueHandling { get; set; } = NullValues.Ignore;

    /// <summary>
    /// Options for serialisation property style.
    /// </summary>
    public PropertyStyle PropertyStyle { get; set; } = PropertyStyle.Unmodified;

    /// <summary>
    /// Settings to determine request failure retry logic.
    /// </summary>
    public RetryPolicy RetryPolicy { get; set; } = new RetryPolicy();
  }

  /// <summary>
  /// Options for handling non-200 HTTP status codes.
  /// </summary>
  public enum NonHttpSuccessCodes
  {
    /// <summary>
    /// The Json client will continue to deserialise the response after receiving a non-success http status code.
    /// </summary>
    Continue = 0,

    /// <summary>
    /// The Json client will throw an HttpRequestException for a non-success http status code.
    /// </summary>
    ThrowException = 1
  }

  /// <summary>
  /// Options for inclusion of null property values.
  /// </summary>
  public enum NullValues
  {
    /// <summary>
    /// Properties with null values will be included in the serialised request content.
    /// </summary>
    Include = 0,

    /// <summary>
    /// Properties with null values will be omitted from the serialised request content.
    /// </summary>
    Ignore = 1
  }

  /// <summary>
  /// Options for serialisation of property names.
  /// </summary>
  public enum PropertyStyle
  {
    /// <summary>
    /// Property names will be (de)serialised as is.
    /// Property level annotations will be respected.
    /// </summary>
    Unmodified = 0,

    /// <summary>
    /// Property names will be converted to camel case when (de)serialised.
    /// </summary>
    CamelCase = 1
  }
}
