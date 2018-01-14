<h1 align="center">KrakenCore</h1>

<h4 align="center">.NET client for <a href="https://www.kraken.com/">Kraken bitcoin exchange API</a></h4>

<p align="center">
    <a href="https://www.nuget.org/packages/KrakenCore">
        <img src="https://img.shields.io/nuget/vpre/KrakenCore.svg" alt="NuGet Package">
    </a>
    <a href="https://ci.appveyor.com/project/discosultan/krakencore">
        <img src="https://img.shields.io/appveyor/ci/discosultan/krakencore.svg?label=windows" alt="AppVeyor Build Status">
    </a>
    <a href="https://travis-ci.org/discosultan/KrakenCore">
        <img src="https://img.shields.io/travis/discosultan/KrakenCore.svg?label=unix" alt="Travis Build Status">
    </a>
</p>

> ⚠ This is an alpha version, meaning the API has not been tested on any production application. USE AT OWN RISK! Also, the API does not include the [tentative private user funding API](https://www.kraken.com/help/api#private-user-funding) as it is subject to change.

## 🎉 Features
- &nbsp;✖ Cross-platform based on [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)
- 🔁 Asynchronous API using [async and await](https://docs.microsoft.com/en-us/dotnet/csharp/async)
- 💪 Strongly typed models
- 🛂 Covered with tests
- 🔐 Supports two-factor authentication

## 📦 Getting Started

```csharp
using KrakenCore;
```

```csharp
var client = new KrakenClient(apiKey, privateKey);

var response = await client.GetAccountBalance();
foreach (var currency in response.Result)
    Console.WriteLine($"{currency.Key} : {currency.Value}");

// ZEUR : 1000
// XXBT : 1
// XETH : 3
```

## 🔧 Extending Client

The client supports two extensibility points: one right before a request to Kraken is dispatched and one right after a response is received. These points provide additional context specific information (for example, the cost of a particular call) and can be used to implement features such as rate limiting or logging.

Sample extensions implemented in [the test suite](https://github.com/discosultan/KrakenCore/blob/6147afa9f7c6ba6ca38c2dee65102594cacb6fe6/tests/KrakenClient.Tests.cs#L46-L67):

```csharp
var client = new KrakenClient(ApiKey, PrivateKey)
{
    InterceptRequest = async req =>
    {
        // Log request.
        output.WriteLine(req.HttpRequest.ToString());
        string content = await req.HttpRequest.Content.ReadAsStringAsync();
        if (!string.IsNullOrWhiteSpace(content)) output.WriteLine(content);

        // Wait if we have hit the API rate limit.
        RateLimiter limiter = req.HttpRequest.RequestUri.OriginalString.Contains("/private/")
            ? privateApiRateLimiter
            : publicApiRateLimiter;
        await limiter.WaitAccess(req.ApiCallCost);
    }
};
```

## 🙏 Related Work

- [C# Kraken API](https://bitbucket.org/arrivets/krakenapi)
- [C# Library to access the Kraken REST API](https://github.com/trenki2/KrakenApi)
