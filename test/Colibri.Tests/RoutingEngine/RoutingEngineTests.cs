using Colibri.Configuration;
using Colibri.Configuration.Models;
using Colibri.Helpers;
using Colibri.Runtime.Pipeline.Main.RoutingEngine;
using Colibri.Runtime.Snapshots;
using Colibri.Runtime.Snapshots.Routing;

namespace Unit.RoutingEngine;

public sealed class RoutingEngineTests
{
    private readonly UpstreamMatcher _matcher = new();
    private readonly DownstreamPathBuilder _pathBuilder = new();
    
    [Theory]
    [InlineData("/")]
    [InlineData("/cluster1/route1/action")]
    [InlineData("/cluster1/route1action1")]
    [InlineData("/route1/action1")]
    [InlineData("/cluster1/route1/action_1")]
    [InlineData("/cluster_1/route1/action1")]
    [InlineData("/some/random/request")]
    [InlineData("/service1/action1")]
    [InlineData("/cluster1/service1/action1")]
    [InlineData("/cluster1/route1/action1/some")]
    public void TryMatch_WhenRequestUriIsInvalid_ShouldReturnFalse(string requestUri)
    {
        // Arrange
        var clusters = new ClusterCfg[]
        {
            new()
            {
                Enabled = true,
                Name = "cluster1",
                Prefix = "/cluster1",
                UsePrefix = true,
                Protocol = "http",
                Hosts = [ "127.0.0.1" ]
            }
        };

        var routes = new RouteCfg[]
        {
            new()
            {
                ClusterName = "cluster1",
                Methods = [ "GET" ],
                UpstreamPattern = "/route1/action1",
                DownstreamPattern = "/service1/action1",
            }
        };

        var snapshot = GetRoutingSnapshot(clusters, routes);
        Span<ParamValue> routeParams = stackalloc ParamValue[16];

        // Act
        var matchResult = _matcher.TryMatch(
            snapshot,
            requestUri.AsSpan(),
            HttpMethodMask.GetMask("GET"),
            routeParams,
            out _,
            out _,
            out _);
        
        // Assert
        Assert.False(matchResult);
    }
    
    [Theory]
    [InlineData("/cluster1/route1/action1")]
    [InlineData("/cluster1/route2/action2")]
    public void TryMatch_WhenRequestUriIsValid_ShouldReturnTrue(string requestUri)
    {
        // Arrange
        var clusters = new ClusterCfg[]
        {
            new()
            {
                Enabled = true,
                Name = "cluster1",
                Prefix = "/cluster1",
                UsePrefix = true,
                Protocol = "http",
                Hosts = [ "127.0.0.1" ]
            }
        };

        var routes = new RouteCfg[]
        {
            new()
            {
                ClusterName = "cluster1",
                Methods = [ "GET" ],
                UpstreamPattern = "/route1/action1",
                DownstreamPattern = "/service1/action1",
            },
            new()
            {
                ClusterName = "cluster1",
                Methods = [ "GET" ],
                UpstreamPattern = "/route2/action2",
                DownstreamPattern = "/action1",
            }
        };

        var snapshot = GetRoutingSnapshot(clusters, routes);
        Span<ParamValue> routeParams = stackalloc ParamValue[16];

        // Act
        var matchResult = _matcher.TryMatch(
            snapshot,
            requestUri.AsSpan(),
            HttpMethodMask.GetMask("GET"),
            routeParams,
            out _,
            out _,
            out _);
        
        // Assert
        Assert.True(matchResult);
    }
    
    [Theory]
    [InlineData("GET", "POST")]
    [InlineData("POST", "GET")]
    [InlineData("PATCH", "PUT")]
    public void TryMatch_WhenMethodIsInvalid_ShouldReturnFalse(string patternMethod, string requestMethod)
    {
        // Arrange
        var clusters = new ClusterCfg[]
        {
            new()
            {
                Enabled = true,
                Name = "cluster1",
                Prefix = "/cluster1",
                UsePrefix = true,
                Protocol = "http",
                Hosts = [ "127.0.0.1" ]
            }
        };

        var routes = new RouteCfg[]
        {
            new()
            {
                ClusterName = "cluster1",
                Methods = [ patternMethod ],
                UpstreamPattern = "/route1/action1",
                DownstreamPattern = "/service1/action1",
            }
        };

        var snapshot = GetRoutingSnapshot(clusters, routes);
        Span<ParamValue> routeParams = stackalloc ParamValue[16];
        string requestUri = "/cluster1/route1/action1";

        // Act
        var matchResult = _matcher.TryMatch(
            snapshot,
            requestUri.AsSpan(),
            HttpMethodMask.GetMask(requestMethod),
            routeParams,
            out _,
            out _,
            out _);
        
        // Assert
        Assert.False(matchResult);
    }
    
    [Theory]
    [InlineData("GET", "GET")]
    [InlineData("POST", "POST")]
    [InlineData("PATCH", "PATCH")]
    public void TryMatch_WhenMethodIsValid_ShouldReturnTrue(string patternMethod, string requestMethod)
    {
        // Arrange
        var clusters = new ClusterCfg[]
        {
            new()
            {
                Enabled = true,
                Name = "cluster1",
                Prefix = "/cluster1",
                UsePrefix = true,
                Protocol = "http",
                Hosts = [ "127.0.0.1" ]
            }
        };

        var routes = new RouteCfg[]
        {
            new()
            {
                ClusterName = "cluster1",
                Methods = [ patternMethod ],
                UpstreamPattern = "/route1/action1",
                DownstreamPattern = "/service1/action1",
            }
        };

        var snapshot = GetRoutingSnapshot(clusters, routes);
        Span<ParamValue> routeParams = stackalloc ParamValue[16];
        string requestUri = "/cluster1/route1/action1";

        // Act
        var matchResult = _matcher.TryMatch(
            snapshot,
            requestUri.AsSpan(),
            HttpMethodMask.GetMask(requestMethod),
            routeParams,
            out _,
            out _,
            out _);
        
        // Assert
        Assert.True(matchResult);
    }
    
    [Theory]
    [InlineData("/cluster1/route1/action1")]
    [InlineData("/cluster1/route2/action2")]
    [InlineData("/cluster2/route3/action3")]
    public void TryMatch_WhenPrefixesAreDisabled_ShouldReturnFalse(string requestUri)
    {
        // Arrange
        var clusters = new ClusterCfg[]
        {
            new()
            {
                Enabled = true,
                Name = "cluster1",
                Prefix = "/cluster1",
                UsePrefix = false,
                Protocol = "http",
                Hosts = [ "127.0.0.1" ]
            },
            new()
            {
                Enabled = true,
                Name = "cluster2",
                Prefix = "/cluster2",
                UsePrefix = false,
                Protocol = "http",
                Hosts = [ "127.0.0.1" ]
            }
        };

        var routes = new RouteCfg[]
        {
            new()
            {
                ClusterName = "cluster1",
                Methods = [ "GET" ],
                UpstreamPattern = "/route1/action1",
                DownstreamPattern = "/service1/action1",
            },
            new()
            {
                ClusterName = "cluster2",
                Methods = [ "GET" ],
                UpstreamPattern = "/route3/action3",
                DownstreamPattern = "/service3/action3",
            },
            new()
            {
                ClusterName = "cluster1",
                Methods = [ "GET" ],
                UpstreamPattern = "/route2/action2",
                DownstreamPattern = "/service2/action2",
            }
        };
        
        var snapshot = GetRoutingSnapshot(clusters, routes);
        Span<ParamValue> routeParams = stackalloc ParamValue[16];
        
        // Act
        var result = _matcher.TryMatch(
            snapshot,
            requestUri.AsSpan(),
            HttpMethodMask.GetMask("GET"),
            routeParams,
            out _,
            out _,
            out _);
        
        // Assert
        Assert.False(result);
    }
    
    [Theory]
    [InlineData("/route1/action1")]
    [InlineData("/route2/action2")]
    [InlineData("/route3/action3")]
    public void TryMatch_WhenPrefixesAreDisabled_ShouldReturnTrue(string requestUri)
    {
        // Arrange
        var clusters = new ClusterCfg[]
        {
            new()
            {
                Enabled = true,
                Name = "cluster1",
                Prefix = "/cluster1",
                UsePrefix = false,
                Protocol = "http",
                Hosts = [ "127.0.0.1" ]
            },
            new()
            {
                Enabled = true,
                Name = "cluster2",
                Prefix = "/cluster2",
                UsePrefix = false,
                Protocol = "http",
                Hosts = [ "127.0.0.1" ]
            }
        };

        var routes = new RouteCfg[]
        {
            new()
            {
                ClusterName = "cluster1",
                Methods = [ "GET" ],
                UpstreamPattern = "/route1/action1",
                DownstreamPattern = "/service1/action1",
            },
            new()
            {
                ClusterName = "cluster2",
                Methods = [ "GET" ],
                UpstreamPattern = "/route3/action3",
                DownstreamPattern = "/service3/action3",
            },
            new()
            {
                ClusterName = "cluster1",
                Methods = [ "GET" ],
                UpstreamPattern = "/route2/action2",
                DownstreamPattern = "/service2/action2",
            }
        };
        
        var snapshot = GetRoutingSnapshot(clusters, routes);
        Span<ParamValue> routeParams = stackalloc ParamValue[16];
        
        // Act
        var result = _matcher.TryMatch(
            snapshot,
            requestUri.AsSpan(),
            HttpMethodMask.GetMask("GET"),
            routeParams,
            out _,
            out _,
            out _);
        
        // Assert
        Assert.True(result);
    }
    
    [Theory]
    [InlineData("/cluster1/route1/action1")]
    [InlineData("/cluster1/route2/action2")]
    [InlineData("/cluster2/route3/action3")]
    public void TryMatch_WhenPrefixesAreEnabled_ShouldReturnTrue(string requestUri)
    {
        // Arrange
        var clusters = new ClusterCfg[]
        {
            new()
            {
                Enabled = true,
                Name = "cluster1",
                Prefix = "/cluster1",
                UsePrefix = true,
                Protocol = "http",
                Hosts = [ "127.0.0.1" ]
            },
            new()
            {
                Enabled = true,
                Name = "cluster2",
                Prefix = "/cluster2",
                UsePrefix = true,
                Protocol = "http",
                Hosts = [ "127.0.0.1" ]
            }
        };

        var routes = new RouteCfg[]
        {
            new()
            {
                ClusterName = "cluster1",
                Methods = [ "GET" ],
                UpstreamPattern = "/route1/action1",
                DownstreamPattern = "/service1/action1",
            },
            new()
            {
                ClusterName = "cluster2",
                Methods = [ "GET" ],
                UpstreamPattern = "/route3/action3",
                DownstreamPattern = "/service3/action3",
            },
            new()
            {
                ClusterName = "cluster1",
                Methods = [ "GET" ],
                UpstreamPattern = "/route2/action2",
                DownstreamPattern = "/service2/action2",
            }
        };
        
        var snapshot = GetRoutingSnapshot(clusters, routes);
        Span<ParamValue> routeParams = stackalloc ParamValue[16];
        
        // Act
        var result = _matcher.TryMatch(
            snapshot,
            requestUri.AsSpan(),
            HttpMethodMask.GetMask("GET"),
            routeParams,
            out _,
            out _,
            out _);
        
        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("/route1/action1")]
    [InlineData("/route2/action2")]
    [InlineData("/cluster2/route3/action3")]
    public void TryMatch_SingleClusterPrefixIsDisabled_ShouldReturnTrue(string requestUri)
    {
        // Arrange
        var clusters = new ClusterCfg[]
        {
            new()
            {
                Enabled = true,
                Name = "cluster1",
                Prefix = "/cluster1",
                UsePrefix = false,
                Protocol = "http",
                Hosts = [ "127.0.0.1" ]
            },
            new()
            {
                Enabled = true,
                Name = "cluster2",
                Prefix = "/cluster2",
                UsePrefix = true,
                Protocol = "http",
                Hosts = [ "127.0.0.1" ]
            }
        };

        var routes = new RouteCfg[]
        {
            new()
            {
                ClusterName = "cluster1",
                Methods = [ "GET" ],
                UpstreamPattern = "/route1/action1",
                DownstreamPattern = "/service1/action1",
            },
            new()
            {
                ClusterName = "cluster2",
                Methods = [ "GET" ],
                UpstreamPattern = "/route3/action3",
                DownstreamPattern = "/service3/action3",
            },
            new()
            {
                ClusterName = "cluster1",
                Methods = [ "GET" ],
                UpstreamPattern = "/route2/action2",
                DownstreamPattern = "/service2/action2",
            }
        };
        
        var snapshot = GetRoutingSnapshot(clusters, routes);
        Span<ParamValue> routeParams = stackalloc ParamValue[16];
        
        // Act
        var result = _matcher.TryMatch(
            snapshot,
            requestUri.AsSpan(),
            HttpMethodMask.GetMask("GET"),
            routeParams,
            out _,
            out _,
            out _);
        
        // Assert
        Assert.True(result);
    }
    
    [Theory]
    [InlineData("/cluster1/route1/action1")]
    [InlineData("/cluster1/route2/action2")]
    [InlineData("/cluster2/route3/action3")]
    [InlineData("/route1/action1")]
    [InlineData("/route2/action2")]
    [InlineData("/route3/action3")]
    public void TryMatch_WhenClustersAreDisabled_ShouldReturnFalse(string requestUri)
    {
        // Arrange
        var clusters = new ClusterCfg[]
        {
            new()
            {
                Enabled = false,
                Name = "cluster1",
                Prefix = "/cluster1",
                UsePrefix = true,
                Protocol = "http",
                Hosts = [ "127.0.0.1" ]
            },
            new()
            {
                Enabled = false,
                Name = "cluster2",
                Prefix = "/cluster2",
                UsePrefix = true,
                Protocol = "http",
                Hosts = [ "127.0.0.1" ]
            }
        };

        var routes = new RouteCfg[]
        {
            new()
            {
                ClusterName = "cluster1",
                Methods = [ "GET" ],
                UpstreamPattern = "/route1/action1",
                DownstreamPattern = "/service1/action1",
            },
            new()
            {
                ClusterName = "cluster2",
                Methods = [ "GET" ],
                UpstreamPattern = "/route3/action3",
                DownstreamPattern = "/service3/action3",
            },
            new()
            {
                ClusterName = "cluster1",
                Methods = [ "GET" ],
                UpstreamPattern = "/route2/action2",
                DownstreamPattern = "/service2/action2",
            }
        };
        
        var snapshot = GetRoutingSnapshot(clusters, routes);
        Span<ParamValue> routeParams = stackalloc ParamValue[16];
        
        // Act
        var result = _matcher.TryMatch(
            snapshot,
            requestUri.AsSpan(),
            HttpMethodMask.GetMask("GET"),
            routeParams,
            out _,
            out _,
            out _);
        
        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void TryMatch_WhenRequestUriIsStatic_ShouldReturnCorrectDownstream()
    {
        // Arrange
        var clusters = new ClusterCfg[]
        {
            new()
            {
                Enabled = true,
                Name = "cluster1",
                Prefix = "/cluster1",
                UsePrefix = true,
                Protocol = "http",
                Hosts = [ "127.0.0.1" ]
            }
        };

        var routes = new RouteCfg[]
        {
            new()
            {
                ClusterName = "cluster1",
                Methods = [ "GET" ],
                UpstreamPattern = "/route1/{id}/action1",
                DownstreamPattern = "/service1/{id}/action1",
            },
            new()
            {
                ClusterName = "cluster1",
                Methods = [ "GET" ],
                UpstreamPattern = "/route1/id/action1",
                DownstreamPattern = "/service1/id/action1",
            }
        };
        
        var snapshot = GetRoutingSnapshot(clusters, routes);
        Span<ParamValue> routeParams = stackalloc ParamValue[16];
        const string requestUri = "/cluster1/route1/id/action1";
        const string expectedUri = "/service1/id/action1";
        
        // Act
        var matchResult = _matcher.TryMatch(
            snapshot,
            requestUri.AsSpan(),
            HttpMethodMask.GetMask("GET"),
            routeParams,
            out _,
            out var downstreamFirstChildIndex,
            out var downstreamChildrenCount);
        
        var pathResult = _pathBuilder.Build(
            snapshot,
            requestUri.AsSpan(),
            routeParams,
            downstreamFirstChildIndex,
            downstreamChildrenCount);
        
        // Assert
        Assert.True(matchResult);
        Assert.DoesNotContain(routeParams.ToArray(), p => p.Length > 0);
        Assert.Equal(expectedUri, pathResult);
    }
    
    [Theory]
    [InlineData(
        "/cluster1/users/vasya/log/18.03.2026",
        "/service1/users/vasya/log/18.03.2026")]
    [InlineData(
        "/cluster1/a/param-1/b/param-2/c/param-3/d/param-4/e/param-5",
        "/s/param-2/p/param-1/o/param-4/n/param-3/m/param-5/l")]
    [InlineData(
        "/cluster2/a/param_1/b/param:2/c/param*3/d/param-4/e/param+5/f/param=6/g/param#7/h/param;8/i/param!9/j/param@10/k",
        "/s/param+5/p/param_1/o/param-4/n/param*3/m/param=6/l/param:2/k/param@10/j/param#7/i/param!9/h/param;8")]
    [InlineData(
        "/cluster1/a/param1/b/param2/c/param3/d/param4/e/param5/f/param6/g/param7/h/param8/i/param9/j/param10/k/param11/l/param12/m/param13/n/param14/o/param15/p/param16/s",
        "/s/param14/p/param1/o/param4/n/param3/m/param5/l/param2/k/param15/j/param16/i/param6/h/param13/g/param7/f/param10/e/param9/d/param11/c/param8/b/a/param12")]
    public void TryMatch_WhenRequestUriHasParams_ShouldReturnCorrectDownstream(string requestUri, string expectedUri)
    {
        // Arrange
        var clusters = new ClusterCfg[]
        {
            new()
            {
                Enabled = true,
                Name = "cluster1",
                Prefix = "/cluster1",
                UsePrefix = true,
                Protocol = "http",
                Hosts = ["127.0.0.1"]
            },
            new()
            {
                Enabled = true,
                Name = "cluster2",
                Prefix = "/cluster2",
                UsePrefix = true,
                Protocol = "http",
                Hosts = ["127.0.0.1"]
            }
        };

        var routes = new RouteCfg[]
        {
            new()
            {
                ClusterName = "cluster1",
                Methods = ["GET"],
                UpstreamPattern = "/route1/action1",
                DownstreamPattern = "/service1/action1",
            },
            new()
            {
                ClusterName = "cluster1",
                Methods = ["GET"],
                UpstreamPattern =
                    "/a/{p1}/b/{p2}/c/{p3}/d/{p4}/e/{p5}/f/{p6}/g/{p7}/h/{p8}/i/{p9}/j/{p10}/k/{p11}/l/{p12}/m/{p13}/n/{p14}/o/{p15}/p/{p16}/s",
                DownstreamPattern =
                    "/s/{p14}/p/{p1}/o/{p4}/n/{p3}/m/{p5}/l/{p2}/k/{p15}/j/{p16}/i/{p6}/h/{p13}/g/{p7}/f/{p10}/e/{p9}/d/{p11}/c/{p8}/b/a/{p12}"
            },
            new()
            {
                ClusterName = "cluster1",
                Methods = ["GET"],
                UpstreamPattern =
                    "/a/{p1}/b/{p2}/c/{p3}/d/{p4}/e/{p5}",
                DownstreamPattern =
                    "/s/{p2}/p/{p1}/o/{p4}/n/{p3}/m/{p5}/l"
            },
            new()
            {
                ClusterName = "cluster2",
                Methods = ["GET"],
                UpstreamPattern =
                    "/a/{p1}/b/{p2}/c/{p3}/d/{p4}/e/{p5}/f/{p6}/g/{p7}/h/{p8}/i/{p9}/j/{p10}/k",
                DownstreamPattern =
                    "/s/{p5}/p/{p1}/o/{p4}/n/{p3}/m/{p6}/l/{p2}/k/{p10}/j/{p7}/i/{p9}/h/{p8}"
            },
            new()
            {
                ClusterName = "cluster1",
                Methods = ["GET"],
                UpstreamPattern = "/route1/action2",
                DownstreamPattern = "/service1/action2"
            },
            new()
            {
                ClusterName = "cluster1",
                Methods = [ "GET" ],
                UpstreamPattern = "/users/{userId}/log/{currentDate}",
                DownstreamPattern = "/service1/users/{userId}/log/{currentDate}",
            }
        };
        
        var snapshot = GetRoutingSnapshot(clusters, routes);
        Span<ParamValue> routeParams = stackalloc ParamValue[16];

        // Act
        var matchResult = _matcher.TryMatch(
            snapshot,
            requestUri.AsSpan(),
            HttpMethodMask.GetMask("GET"),
            routeParams,
            out _,
            out var downstreamFirstChildIndex,
            out var downstreamChildrenCount);
        
        var pathResult = _pathBuilder.Build(
            snapshot,
            requestUri.AsSpan(),
            routeParams,
            downstreamFirstChildIndex,
            downstreamChildrenCount);
        
        // Assert
        Assert.True(matchResult);
        Assert.Equal(expectedUri, pathResult);
    }
    
    private RoutingSnapshot GetRoutingSnapshot(ClusterCfg[] clusters, RouteCfg[] routes)
    {
        var settings = new ColibriSettings
        {
            Clusters = clusters,
            Routes = routes
        };

        return new GlobalSnapshotBuilder()
            .Build(settings)
            .RoutingSnapshot;
    }
}