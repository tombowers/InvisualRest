using System;
using System.Text;

namespace InvisualRest.Configuration
{
  /// <summary>
  /// This class wraps authentication information for JsonRestClientOptions.
  /// </summary>
  public class AuthenticationInfo
  {
    private readonly string _username;
    private readonly string _password;

    /// <summary>
    /// Creates an instance of <see cref="AuthenticationInfo"/>.
    /// </summary>
    public AuthenticationInfo(string username, string password)
    {
      if (string.IsNullOrWhiteSpace(username)) throw new ArgumentNullException(nameof(username));
      if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password));

      _username = username;
      _password = password;
    }

    /// <summary>
    /// This method is overridden to provide a string correctly formatted for use as a header value for HTTP authentication.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      var combinedKey = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_username}:{_password}"));
      
      return "Basic " + combinedKey;
    }
  }
}
