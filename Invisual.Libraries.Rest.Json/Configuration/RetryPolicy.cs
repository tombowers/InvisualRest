using System.Collections.Generic;

namespace InvisualRest.Configuration
{
  /// <summary>
  /// Provides options for configuration of request retry logic.
  /// </summary>
  public class RetryPolicy
  {
    /// <summary>
    /// The maximum number of times to retry failed requests.
    /// </summary>
    public int MaxRetries { get; set; } = 10;

    /// <summary>
    /// The time to wait between request retries. This setting is only used when the DelayInterval is set to 'SetInterval'.
    /// </summary>
    public int SetDelayMilliseconds { get; set; }

    /// <summary>
    /// The type of interval to use between request retries.
    /// </summary>
    public RetryDelayInterval DelayInterval { get; set; }

    /// <summary>
    /// A collection of HTTP status codes given to indicate a failed request.
    /// </summary>
    public List<int> HttpStatuses { get; set; } = new List<int>();

    /// <summary>
    /// Retry on all exceptions.
    /// </summary>
    public bool OnException { get; set; }

    /// <summary>
    /// Options for the type of interval to use between requests.
    /// </summary>
    public enum RetryDelayInterval
    {
      /// <summary>
      /// A fixed time interval between requests.
      /// </summary>
      SetInterval = 0,

      /// <summary>
      /// The time between retries gets progressively and exponentially longer.
      /// </summary>
      ExponentialBackoff = 1
    }
  }
}
