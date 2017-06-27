using System;
using System.Collections.Generic;

namespace InvisualRest.Configuration
{
  public class RetryPolicy
  {
    /// <summary>
    /// Maximum number of times to retry failed requests
    /// </summary>
    public int MaxRetries { get; set; } = 10;

    public int SetDelayMilliseconds { get; set; }
    public RetryDelayInterval DelayInterval { get; set; }

    /// <summary>
    /// A set of flags used to indicate what constitues a request which needs retrying
    /// </summary>
    public Retry WhenToRetry { get; set; }

    /// <summary>
    /// If the WhenToRetry property contains the OnSpecificResponseStatuses flag, this collection must specify those statuses.
    /// </summary>
    public List<int> HttpStatuses { get; set; } = new List<int>();

    [Flags]
    public enum Retry
    {
      Never = 0,
      OnAllExceptions = 1,
      OnSpecificResponseStatuses = 2
    }

    public enum RetryDelayInterval
    {
      None = 0,
      SetInterval = 1,
      ExponentialBackoff = 2
    }
  }
}
