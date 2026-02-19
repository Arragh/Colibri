# 🐥 Colibri 🐥

[![NuGet](https://img.shields.io/nuget/v/Colibri.svg?style=flat-square)](https://www.nuget.org/packages/Colibri/)

AOT-friendly API Gateway based on ASP.NET 10

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

## 📝 Integration into your project:
```csharp
...
builder.Services.AddColibriServices();
...
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
