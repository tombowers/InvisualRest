# InvisualRest
Make rest requests with strongly typed requests and responses. Includes preset and configurable retry policies.

* [Features]
* [Installation]
* [Examples]
  * [Simple GET]
  * [Simple GET - Resource List]
  * [Simple POST]
* [Basic HTTP Authentication]
* [Custom Headers]
* [Retry Logic]
  * [Retry every 1 second]
  * [Exponential Backoff]
  * [Limit Number of Attempts]
  * [Retry on Specified HTTP Status codes]
  * [Retry On Exceptions]
* [Status Code Handling]
* [All Configuration Options]

## Features
* Supports GET, POST, PATCH, PUT, DELETE
* Fully Asynchronous
* Basic HTTP Authentication
* Customisation of HTTP headers
* Uses Json.NET to deserialise to strongly typed response objects
* Built-in customisable retry logic
* Many configuration options allowing compatibility with most REST APIs.

## Installation

To install InvisualRest, run the following command in the [Package Manager Console](https://docs.nuget.org/docs/start-here/using-the-package-manager-console)
```
PM> Install-Package Invisual.Libraries.Rest.Json
```

## Examples

The examples here aren't just made up code. They all use the excellent free demo REST api at https://jsonplaceholder.typicode.com and will actually work.

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

#### Simple POST
```C#
var client = new JsonRestClient("https://jsonplaceholder.typicode.com");

Post newBlogPost = await client.PostAsync<Post>(
    "posts",
    new Post
    {
        Title = "foo",
        UserId = "1",
        Body = "bar"
    });
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
By default, requests will not be retried.
Simply pass a `RetryPolicy` instance into the `JsonRestClient` constructor to enable retry logic.
There are a number of convenience extension methods to help setup advanced retry logic without too much faff.


##### Retry every 1 second
```C#
var client = new JsonRestClient(
    "https://jsonplaceholder.typicode.com",
    new JsonRestClientOptions
    {
        RetryPolicy = Retry.Every(1000)
    });
```

##### Exponential Backoff
Progressively lengthen the time between retries.
```C#
var client = new JsonRestClient(
    "https://jsonplaceholder.typicode.com",
    new JsonRestClientOptions
    {
        RetryPolicy = Retry.WithExponentialBackoff()
    });
```


##### Limit Number of Attempts
Stop retrying after n attempts.
```C#
var client = new JsonRestClient(
    "https://jsonplaceholder.typicode.com",
    new JsonRestClientOptions
    {
        RetryPolicy = Retry.Every(1000).StopAfter(5)
    });
```


##### Retry on Specified HTTP Status codes
```C#
var client = new JsonRestClient(
    "https://jsonplaceholder.typicode.com",
    new JsonRestClientOptions
    {
        RetryPolicy = Retry.Every(1000).On(429, 500)
    });
```

##### Retry On Exceptions
```C#
var client = new JsonRestClient(
    "https://jsonplaceholder.typicode.com",
    new JsonRestClientOptions
    {
        RetryPolicy = Retry
            .Every(1000)
            .OnException()
            .StopAfter(5)
    });
```
It is recommended to use `OnException()` carefully. Exceptions may be caused by non-transient errors, such as a bug or long term outage.
It is recommended practice to chain a call to `OnException` with `StopAfter()` and/or `WithExponentialBackoff()`.

#### Status Code Handling
REST APIs around the web differ in their error state handling.
Some will return a 404 status code for a missing resource, whereas others may return a 200 status code, but include error information in the response.
Others still may do a combination of these.
By default InvisualRest will attempt to deserialise every response, regardless of status. Sometimes, this behaviour may not be desired, so it can be disabled.
This is best shown with examples.

The https://jsonplaceholder.typicode.com API returns a 404 status code for a missing resource, with the following JSON.

```JSON
{}
```

The default InvisualRest options will deserialise this to a default version of your supplied type. E.g.

![Blank Resource](https://raw.githubusercontent.com/tombowers/InvisualRest/master/DocumentationAssets/default-entity.png)

In a lot of cases, this is not desirable. Is this a resource with null values, or a missing resource?
Since the response does not contain any information about a valid resource, it would be better to not deserialise the response, and handle this state unambiguously, throwing an exception for all non-200 statuses.

```C#
var client = new JsonRestClient(
    "https://jsonplaceholder.typicode.com",
    new JsonRestClientOptions
    {
        NonHttpSuccessCodeHandling = NonHttpSuccessCodes.ThrowException
    });
```

Now, all unsuccessful requests will throw a `RestException`.
This exception contains an `HttpStatusCode` and a `RawResponse` property in case you need more specific information about the response.

If an API provider returns error information with each response, and with a non-200 status. E.g.

```JSON
{
    "errorCode": "4b",
    "errorMessage": "Resource not found" 
}
```

You can handle this in two ways. Configure InvisualRest to throw an exception as above, and use the `HttpStatusCode` property to control your logic.
You can use the `RawResponse` property of the exception to access the reponse if you need more info.
Alternatively, you could include the error properties on all of your response types, and allow InvisualRest to deserialise the response, regardless of status code.
Here is an example of one way to implement such a response class.

```C#
public abstract class Response
{
    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }

    public bool HasError => return ErrorCode != null;
}

public class Post : Response
{
    public string UserId { get; set; }
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
}
```

#### All Configuration Options
As seen above with the retry logic, you can pass a `JsonRestClientOptions` object into the `JsonRestClient` constructor. Here are all available options.

| Property | Type | Default Value | Available Values | Notes |
| --- | --- | --- | --- |
| AuthenticationInfo  | AuthenticationInfo  | null | - | To provide credentials for Basic HTTP Authentication. See [Basic HTTP Authentication] |
| NonHttpSuccessCodeHandling  | enum [NonHttpSuccessCodes]  | Continue | Continue, ThrowException | Choose to throw an exception when a non-200 HTTP status code is received. See [Status Code Handling]. |
| NullValueHandling | enum [NullValues] | Include | Include, Ignore | Choose whether or not to include properties with null values in the serialised request, or omit them altogether. |
| PropertyStyle | enum [PropertyStyle] | Unmodified | Unmodified, CamelCase | Serialise/Deserialise property names as-is, or convert to/from camel case. Property-level Json.NET attributes will be respected. |
| RetryPolicy | RetryPolicy | null | - | See [Retry Logic] |