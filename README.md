# 🐥 Colibri 🐥

[![NuGet](https://img.shields.io/nuget/v/Colibri.svg?logo=nuget&label=NuGet&color=green)](https://www.nuget.org/packages/Colibri/)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

Colibri is a fast, lightweight, easy-to-configure, AOT-friendly API gateway that doesn't overload you with heavy or unnecessary features.
Project is in an early stage, please be understanding.

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
         <td align="center">+</td>
         <td>Enable or disable the cluster. If the cluster is disabled, all its routes become unavailable.</td>
      </tr>
      <tr>
         <td>ClusterId</td>
         <td><code>string</code></td>
         <td align="center">+</td>
         <td>Used exclusively for internal logic.</td>
      </tr>
      <tr>
         <td>Protocol</td>
         <td><code>string</code></td>
         <td align="center">+</td>
         <td>Connection protocol. Allowed values: 'http' and 'ws'. Must be lowercase.</td>
      </tr>
      <tr>
         <td>Prefix</td>
         <td><code>string</code></td>
         <td align="center">+</td>
         <td>Route prefix. Must be in the format '/yourprefixname' — a single segment starting with '/' and no longer than 250 characters. When enabled, all routes in this cluster will be prefixed. For example, if a route pattern is '/users', the final route becomes '/yourprefixname/users'.</td>
      </tr>
      <tr>
         <td>UsePrefix</td>
         <td><code>bool</code></td>
         <td align="center">-</td>
         <td>Can be disabled, in which case all routes in this cluster will not use the prefix.</td>
      </tr>
      <tr>
         <td>Hosts</td>
         <td><code>string[]</code></td>
         <td align="center">+</td>
         <td>List of available hosts for this cluster.</td>
      </tr>
   </tbody>
</table>
