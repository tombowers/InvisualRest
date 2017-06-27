using System;
using System.Text;

namespace InvisualRest.Configuration
{
  public class AuthenticationInfo
  {
    private readonly string _username;
    private readonly string _password;

    public AuthenticationInfo(string username, string password)
    {
      if (string.IsNullOrWhiteSpace(username)) throw new ArgumentNullException(nameof(username));
      if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password));

      _username = username;
      _password = password;
    }

    public override string ToString()
    {
      var combinedKey = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_username}:{_password}"));
      
      return "Basic " + combinedKey;
    }
  }
}
