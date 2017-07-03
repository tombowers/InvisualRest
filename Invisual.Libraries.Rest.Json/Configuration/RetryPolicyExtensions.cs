namespace InvisualRest.Configuration
{
  /// <summary>
  /// Extension methods for configuration of custom request retry policies.
  /// </summary>
  public static class Retry
  {
    /// <summary>
    /// Specify HTTP status codes which indicate request failure.
    /// </summary>
    public static RetryPolicy On(params int[] responseStatusCode)
    {
      return new RetryPolicy().On(responseStatusCode);
    }

    /// <summary>
    /// Specify HTTP status codes which indicate request failure.
    /// </summary>
    public static RetryPolicy On(this RetryPolicy policy, params int[] responseStatusCode)
    {
      policy.HttpStatuses.AddRange(responseStatusCode);

      return policy;
    }

    /// <summary>
    /// Retry on all exceptions.
    /// </summary>
    public static RetryPolicy OnException()
    {
      return new RetryPolicy().OnException();
    }

    /// <summary>
    /// Retry on all exceptions.
    /// </summary>
    public static RetryPolicy OnException(this RetryPolicy policy)
    {
      policy.OnException = true;

      return policy;
    }

    /// <summary>
    /// Retry every n milliseconds.
    /// </summary>
    public static RetryPolicy Every(int millseconds)
    {
      return new RetryPolicy().Every(millseconds);
    }

    /// <summary>
    /// Retry every n milliseconds.
    /// </summary>
    public static RetryPolicy Every(this RetryPolicy policy, int millseconds)
    {
      policy.DelayInterval = RetryPolicy.RetryDelayInterval.SetInterval;
      policy.SetDelayMilliseconds = millseconds;

      return policy;
    }

    /// <summary>
    /// Retry with an exponentially increasing interval.
    /// </summary>
    public static RetryPolicy WithExponentialBackoff()
    {
      return new RetryPolicy().WithExponentialBackoff();
    }

    /// <summary>
    /// Retry with an exponentially increasing interval.
    /// </summary>
    public static RetryPolicy WithExponentialBackoff(this RetryPolicy policy)
    {
      policy.DelayInterval = RetryPolicy.RetryDelayInterval.ExponentialBackoff;

      return policy;
    }

    /// <summary>
    /// Limit the number of potential request retries.
    /// </summary>
    public static RetryPolicy StopAfter(int tries)
    {
      return new RetryPolicy().StopAfter(tries);
    }

    /// <summary>
    /// Limit the number of potential request retries.
    /// </summary>
    public static RetryPolicy StopAfter(this RetryPolicy policy, int tries)
    {
      policy.MaxRetries = tries;

      return policy;
    }
  }
}
