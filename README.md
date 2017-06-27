# InvisualRest
Make rest requests with strongly typed requests and responses. Includes preset and configurable retry policies.

## Overview
Simple GET request
```C#
var client = new JsonRestClient("https://jsonplaceholder.typicode.com");

Post post = await client.GetAsync<Post>("posts/1");
```

## Installation

To install InvisualRest, run the following command in the [Package Manager Console](https://docs.nuget.org/docs/start-here/using-the-package-manager-console)
```
PM> Install-Package Invisual.Libraries.Rest.Json
```



