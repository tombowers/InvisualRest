namespace InvisualRest.Configuration
{
  public static class RetryPolicyExtensions
  {
    public static RetryPolicy RetryOn(this RetryPolicy policy, params int[] responseStatusCode)
    {
      policy.WhenToRetry |= RetryPolicy.Retry.OnSpecificResponseStatuses;
      policy.HttpStatuses.AddRange(responseStatusCode);

      return policy;
    }

    public static RetryPolicy RetryEvery(this RetryPolicy policy, int millseconds)
    {
      policy.DelayInterval = RetryPolicy.RetryDelayInterval.SetInterval;
      policy.SetDelayMilliseconds = millseconds;

      return policy;
    }

    public static RetryPolicy RetryWithExponentialBackoff(this RetryPolicy policy)
    {
      policy.DelayInterval = RetryPolicy.RetryDelayInterval.ExponentialBackoff;

      return policy;
    }

    public static RetryPolicy StopAt(this RetryPolicy policy, int tries)
    {
      policy.MaxRetries = tries;

      return policy;
    }
  }
}
