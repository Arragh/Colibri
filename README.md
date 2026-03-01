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
10) Configuration validation on start/hot-reload.
11) Authorization per cluster (early stage implementation, token only - not depends any roles or claims).

## 🔧 In development:
1) Rate Limiter.
2) Unit tests.
3) Authorization improvements.

## ⚙️ Technical Limitations
1) Maximum length of a single route segment: 250 characters.
2) Total maximum number of segments length across all routes (including cluster prefixes): 65,535.
3) Total clusters count limit: 5000
4) Total routes count limit: 5000

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
      "Clusters": [
         {
            "Enabled": true,
            "Name": "Cluster1", // Unique cluster identifier
            "Protocol": "http", // or "ws"
            "Prefix": "/cluster1", // Route prefix (http://cluster1/service1/method1)
            "UsePrefix": true, // Whether to apply the prefix for routes
            "Hosts": [ "192.168.1.100:6000", "192.168.1.102:6002" ],
            "Authorization": {
               "Enabled": true, // Disabled by default
               "Algorithm": "rs256", // Algorithm used for key. Supports rs256, hs256, es256
               "Key": "" // token key (public/simmetrical/etc)
            },
            "Retry": {
               "Enabled": true, // Disabled by default
               "Attempts": 3 // Number of retry attempts. 3 by deafult
            },
            "LoadBalancer": {
               "Enabled": true, // Disabled by default
               "Type": "RR" // RND - random, RR - round-robin. RR by default
            },
            "CircuitBreaker": {
               "Enabled": true, // Disabled by default
               "Failures": 5, // Number of consecutive failures to trigger, 5 by default
               "Timeout": 30 // Seconds to block the host. 30 by defaul
            }
         },
         {
            // Simplified cluster setting without load-balancer, retrier and circuit breaker
            "Enabled": true,
            "Name": "Cluster2",
            "Protocol": "http",
            "Prefix": "/cluster2",
            "UsePrefix": false,
            "Hosts": [ "192.168.1.100:6000", "192.168.1.102:6002" ]
         }
      ],
      "Routes": [
         {
            "ClusterName": "Cluster1", // Which cluster rule to use
            "Methods": [ "GET" ],
            "UpstreamPattern": "/service1/method1", // External route
            "DownstreamPattern": "/method1" // Internal route
         },
         {
            "ClusterName": "Cluster2",
            "Methods": [ "POST", "DELETE" ], // Multiple methods allowed
            "UpstreamPattern": "/service2/method2/{id}", // Up to 16 parameters supported
            "DownstreamPattern": "/method2/{id}" // Parameters passed to internal service
         }
      ]
   }
}
```
