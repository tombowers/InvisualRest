using System;

namespace InvisualRest
{
  /// <summary>
  /// InvisualRest Exception
  /// </summary>
  [Serializable]
  public class RestException : Exception
  {
    /// <summary>
    /// Creates a <see cref="RestException"/> instance.
    /// </summary>
    public RestException() { }

    /// <summary>
    /// Creates a <see cref="RestException"/> instance.
    /// </summary>
    public RestException(string message) : base(message) { }

    /// <summary>
    /// Creates a <see cref="RestException"/> instance.
    /// </summary>
    public RestException(string message, Exception inner) : base(message, inner) { }

    /// <summary>
    /// The HTTP Status Code returned by the response related to this exception.
    /// This may be null if no response was received.
    /// </summary>
    public int HttpStatusCode { get; set; }

    /// <summary>
    /// The entire HTTP response body, if present.
    /// This may be null if no response was received.
    /// </summary>
    public string RawResponse { get; set; }
  }
}
