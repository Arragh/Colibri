# 🐥 Colibri 🐥

[![NuGet](https://img.shields.io/nuget/v/Colibri.svg?logo=nuget&label=NuGet&color=green)](https://www.nuget.org/packages/Colibri/)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Build Status](https://img.shields.io/github/actions/workflow/status/Arragh/Colibri/publish.yml?branch=master)](https://github.com/Arragh/Colibri/actions)

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