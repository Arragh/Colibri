# 🐥 Colibri 🐥

[![NuGet](https://img.shields.io/nuget/v/Colibri.svg?logo=nuget&label=NuGet&color=green)](https://www.nuget.org/packages/Colibri/)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

AOT-friendly API-Gateway library for ASP.NET

This project is in an early stage, please be understanding.

## 🚀 Features:
1) Custom Trie-based routing system using structures and flat arrays.
2) Supports up to 16 route parameters.
3) Protocol support: HTTP, WebSocket.
4) Proxy requests to internal services.
5) Flexible configuration of routes and methods.
6) Retry requests on failure.
7) Load balancing: Round-Robin, Random.
8) Hot reload of configuration.
9) Circuit Breaker support.

## 🔧 In development:
1) Authorization per cluster.
2) Rate Limiter.
3) Configuration validation.
4) Unit tests.

## ⚙️ Technical Limitations
1) Maximum length of a single route segment: 250 characters.
2) Total maximum number of segments across all routes (including cluster prefixes): 65,535.
    - Assuming an average of 10 segments per route (which is already high for most APIs), this allows for approximately **6,553 routes**.
    - In practice, routes are typically shorter (3–6 segments), so the effective route capacity is significantly higher.
    - It is highly unlikely you will hit this limit in real-world scenarios.

<table>
   <thead>
      <tr>
         <th colspan="4">Clusters</th>
      </tr>
      <tr>
         <th>Parameter</th>
         <th>Type</th>
         <th>Required</th>
         <th>Description</th>
      </tr>
   </thead>
   <tbody>
      <tr>
         <td>Enabled</td>
         <td><code>bool</code></td>
         <td>+</td>
         <td>Включить/Выключить кластер. Если кластер выключен, то все его маршруты будут недоступны.</td>
      </tr>
      <tr>
         <td>ClusterId</td>
         <td><code>string</code></td>
         <td>+</td>
         <td>Используется исключительно для внутренней логики.</td>
      </tr>
      <tr>
         <td>Protocol</td>
         <td><code>string</code></td>
         <td>+</td>
         <td>Connection protocol. Разрешены http и ws. Должен быть в нижнем регистре.</td>
      </tr>
      <tr>
         <td>Prefix</td>
         <td><code>string</code></td>
         <td>+</td>
         <td>Префикс в маршруте. Должен иметь вид '/yourprefixname' - один сегмент, начинающийся с символа '/' и иметь длину не более 250 символов. С включенным префиксом все маршруты данного кластера будут начинаться с префикса. Если route имеет паттерн '/users', то итоговый маршрут будет иметь вид '/yourprefixname/users'</td>
      </tr>
      <tr>
         <td>UsePrefix</td>
         <td><code>bool</code></td>
         <td>-</td>
         <td>Можно отключить, тогда все маршруты данного кластера будут без префикса.</td>
      </tr>
      <tr>
         <td>Hosts</td>
         <td><code>string[]</code></td>
         <td>+</td>
         <td>Доступные хосты для данного кластера.</td>
      </tr>
   </tbody>
</table>

## 📝 Integration into your project:
```csharp
...
builder.Services.AddColibriServices();
...
app.UseWebSockets(); // Only if you want websocket proxy
app.UseColibri();
...
```

## 📝 Example configuration:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Colibri": {
    "Routing": {
      "Clusters": [
        {
          "Enabled": true,
          "ClusterId": "Cluster1", // Unique cluster identifier
          "Protocol": "http", // or "ws"
          "Prefix": "/cluster1", // Route prefix (http://cluster1/service1/method1)
          "UsePrefix": true, // Whether to apply the prefix for routes
          "Hosts": [ "192.168.1.100:6000", "192.168.1.102:6002" ],
          "Retry": {
            "Enabled": true,
            "Attempts": 3 // Number of retry attempts
          },
          "LoadBalancing": {
            "Enabled": true,
            "Type": "RND" // RND - random, RR - round-robin
          },
          "CircuitBreaker": {
            "Enabled": true,
            "Failures": 5, // Number of consecutive failures to trigger
            "Timeout": 30 // Seconds to block the host
          }
        }
      ],
      "Routes": [
        {
          "ClusterId": "Cluster1", // Which cluster’s rules to use
          "Methods": [ "GET" ],
          "UpstreamPattern": "/service1/method1", // External route
          "DownstreamPattern": "/method1" // Internal route
        },
        {
          "ClusterId": "Cluster1",
          "Methods": [ "POST", "DELETE" ], // Multiple methods allowed
          "UpstreamPattern": "/service1/method2/{id}", // Up to 16 parameters supported
          "DownstreamPattern": "/method2/{id}" // Parameters passed to internal service
        }
      ]
    }
  }
}
```
