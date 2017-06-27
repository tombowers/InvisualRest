# InvisualRest
Make rest requests with strongly typed requests and responses. Includes preset and configurable retry policies.

## Features
* Supports GET, POST, PATCH, PUT, DELETE
* Basic HTTP Authentication
* Customisation of HTTP headers
* Uses Json.NET to deserialise to strongly typed response objects
* Built-in customisable retry logic

## Installation

To install InvisualRest, run the following command in the [Package Manager Console](https://docs.nuget.org/docs/start-here/using-the-package-manager-console)
```
PM> Install-Package Invisual.Libraries.Rest.Json
```

## Examples

#### Simple GET
```C#
var client = new JsonRestClient("https://jsonplaceholder.typicode.com");

Post post = await client.GetAsync<Post>("posts/1");

public class Post
{
    public string UserId { get; set; }
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
}
```

#### Simple GET - Resource List
```C#
var client = new JsonRestClient("https://jsonplaceholder.typicode.com");

List<Post> post = await client.GetAsync<List<Post>>("posts");

public class Post
{
    public string UserId { get; set; }
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
}
```

#### Basic HTTP Authentication
```C#
var client = new JsonRestClient(
    "https://jsonplaceholder.typicode.com",
    new JsonRestClientOptions
    {
        AuthenticationInfo = new AuthenticationInfo("username", "password")
    });
```

#### Custom Headers
```C#
var client = new JsonRestClient("https://jsonplaceholder.typicode.com");

client.RequestHeaders.Add("Authorization", "Bearer NDQ3OGViN2FjMTM4YTEzNjg1MmJhYmQ4NjE5NTZjMTk6M2U1YTZlZGVjNzFlYWIwMzk0MjJjNjQ0NGQwMjY1OWQ=");
```


#### Retry Logic
##### Exponential Backoff
Progressively lengthen the time between retries.
```C#
var client = new JsonRestClient(
    "https://jsonplaceholder.typicode.com",
    new JsonRestClientOptions
    {
        RetryPolicy = new RetryPolicy()
            .RetryWithExponentialBackoff()
    });
```


##### Limit Number of Attempts
Stop retrying after n attempts
```C#
var client = new JsonRestClient(
    "https://jsonplaceholder.typicode.com",
    new JsonRestClientOptions
    {
        RetryPolicy = new RetryPolicy()
            .StopAt(5)
    });
```


##### Only Retry on Specified HTTP Status codes
```C#
var client = new JsonRestClient(
    "https://jsonplaceholder.typicode.com",
    new JsonRestClientOptions
    {
        RetryPolicy = new RetryPolicy()
            .RetryOn(429, 500)
    });
```


##### Retry Extension Method Chaining
Retry logic can be chained for complex retry policies
```C#
var client = new JsonRestClient(
    "https://jsonplaceholder.typicode.com",
    new JsonRestClientOptions
    {
        RetryPolicy = new RetryPolicy()
            .RetryOn(500)
            .RetryWithExponentialBackoff()
            .StopAt(5)
    });
```
