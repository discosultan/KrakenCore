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

## 🎉 Features
- &nbsp;✖ Cross-platform based on [.NET Standard 1.6](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)
- 🔁 Asynchronous API using [async and await](https://docs.microsoft.com/en-us/dotnet/csharp/async)
- 💪 Strongly typed models
- 🛂 Covered with tests
- &nbsp;✋ Supports API rate limiter
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

## 🙏 Related Work

- [C# Kraken API](https://bitbucket.org/arrivets/krakenapi)
- [C# Library to access the Kraken REST API](https://github.com/trenki2/KrakenApi)
