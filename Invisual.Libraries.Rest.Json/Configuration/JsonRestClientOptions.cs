namespace InvisualRest.Configuration
{
  public class JsonRestClientOptions
  {
    public AuthenticationInfo AuthenticationInfo { get;set; }
    public NonHttpSuccessCodes NonHttpSuccessCodeHandling { get; set; } = NonHttpSuccessCodes.Continue;
    public NullValues NullValueHandling { get; set; } = NullValues.Ignore;
    public PropertyStyle PropertyStyle { get; set; } = PropertyStyle.Unmodified;
    public RetryPolicy RetryPolicy { get; set; } = new RetryPolicy();
  }

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
