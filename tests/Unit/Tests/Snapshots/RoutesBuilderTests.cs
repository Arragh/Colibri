using Colibri.Configuration.Models;
using Colibri.Runtime.Snapshots.Routing;

namespace Unit.Tests.Snapshots;

public sealed class RoutesBuilderTests
{
    private readonly RoutesBuilder _builder = new();
    
    private ClusterCfg CreateDummyCluster(string name, string prefix, string protocol, string[] hosts, string lbType)
    {
        return new()
        {
            Enabled = false,
            UsePrefix = false,
            Name = name,
            Protocol = protocol,
            Prefix = prefix,
            Hosts = hosts,
            LoadBalancing = new LoadBalancingCfg
            {
                Enabled = false,
                Type = lbType
            },
            CircuitBreaker = new CircuitBreakerCfg
            {
                Enabled = false,
                Failures = 3,
                Timeout = 30
            },
            Retry = new RetryCfg
            {
                Enabled = false,
                Attempts = 5
            }
        };
    }

    private RouteCfg CreateDummyRoute(string[] methods, string clusterName, string upstream, string downstream)
    {
        return new()
        {
            Methods = methods,
            ClusterName = clusterName,
            UpstreamPattern = upstream,
            DownstreamPattern = downstream
        };
    }

    [Fact]
    public void Build_WhenClustersAreEmptyRoutesAreEmpty_ShouldBeEmpty()
    {
        // Arrange
        var clusters = Array.Empty<ClusterCfg>();
        var routes = Array.Empty<RouteCfg>();
        
        // Act
        var result = _builder.Build(clusters, routes);
        
        // Assert
        Assert.Empty(result);
    }
    
    [Fact]
    public void Build_WhenClusterUsePrefixIsFalse_ShouldNotAddPrefixChunk()
    {
        // Arrange
        var cluster = CreateDummyCluster(
            name: "cluster1",
            prefix: "/cluster1",
            protocol: "http",
            hosts: [ "127.0.0.1" ],
            lbType: "rr");

        var route = CreateDummyRoute(
            methods: [ "GET" ],
            clusterName: cluster.Name,
            upstream: "/users/all",
            downstream: "/all");
        
        cluster.UsePrefix = false;
        
        var clusters = new[] { cluster };
        var routes = new[] { route };
        
        // Act
        var result = _builder.Build(clusters, routes);
        
        // Assert
        Assert.Equal(2, result[0].TotalUpstreamChunks.Count);
    }
    
    [Fact]
    public void Build_WhenClusterUsePrefixIsTrue_ShouldAddPrefixChunk()
    {
        // Arrange
        var cluster = CreateDummyCluster(
            name: "cluster1",
            prefix: "/cluster1",
            protocol: "http",
            hosts: [ "127.0.0.1" ],
            lbType: "rr");

        var route = CreateDummyRoute(
            methods: [ "GET" ],
            clusterName: cluster.Name,
            upstream: "/users/all",
            downstream: "/all");
        
        cluster.UsePrefix = true;
        
        var clusters = new[] { cluster };
        var routes = new[] { route };
        
        // Act
        var result = _builder.Build(clusters, routes);
        
        // Assert
        Assert.Equal(3, result[0].TotalUpstreamChunks.Count);
    }
    
    [Fact]
    public void Build_WhenClusterUsePrefixIsFalse_ShouldNotIncludeInUpstreamSegments()
    {
        // Arrange
        var cluster = CreateDummyCluster(
            name: "cluster1",
            prefix: "/cluster1",
            protocol: "http",
            hosts: [ "127.0.0.1" ],
            lbType: "rr");

        var route = CreateDummyRoute(
            methods: [ "GET" ],
            clusterName: cluster.Name,
            upstream: "/users/all",
            downstream: "/all");
        
        cluster.UsePrefix = false;
        
        var clusters = new[] { cluster };
        var routes = new[] { route };
        
        // Act
        var result = _builder.Build(clusters, routes);
        var upstreamSegments = result[0].UpstreamSegments;
        
        // Assert
        Assert.DoesNotContain("cluster1", upstreamSegments);
    }
    
    [Fact]
    public void Build_WhenClusterUsePrefixIsTrue_ShouldNotIncludeInUpstreamSegments()
    {
        // Arrange
        var cluster = CreateDummyCluster(
            name: "cluster1",
            prefix: "/cluster1",
            protocol: "http",
            hosts: [ "127.0.0.1" ],
            lbType: "rr");

        var route = CreateDummyRoute(
            methods: [ "GET" ],
            clusterName: cluster.Name,
            upstream: "/users/all",
            downstream: "/all");
        
        cluster.UsePrefix = true;
        
        var clusters = new[] { cluster };
        var routes = new[] { route };
        
        // Act
        var result = _builder.Build(clusters, routes);
        var upstreamSegments = result[0].UpstreamSegments;
        
        // Assert
        Assert.DoesNotContain("cluster1", upstreamSegments);
    }
    
    [Fact]
    public void Build_WhenClusterUsePrefixIsTrue_ShouldBeFirstChunk()
    {
        // Arrange
        var cluster = CreateDummyCluster(
            name: "cluster1",
            prefix: "/cluster1",
            protocol: "http",
            hosts: [ "127.0.0.1" ],
            lbType: "rr");

        var route = CreateDummyRoute(
            methods: [ "GET" ],
            clusterName: cluster.Name,
            upstream: "/users/all",
            downstream: "/all");
        
        cluster.UsePrefix = true;
        
        var clusters = new[] { cluster };
        var routes = new[] { route };
        
        // Act
        var result = _builder.Build(clusters, routes);
        var clusterPrefixName = cluster.Prefix[1..];
        var upstreamChunkName = result[0].TotalUpstreamChunks[0].Name;
        
        // Assert
        Assert.Equal(clusterPrefixName, upstreamChunkName);
    }

    [Fact]
    public void Build_WhenRouteHasSingleMethod_ShouldBeSingle()
    {
        // Arrange
        var cluster = CreateDummyCluster(
            name: "cluster1",
            prefix: "/cluster1",
            protocol: "http",
            hosts: [ "127.0.0.1" ],
            lbType: "rr");

        var route = CreateDummyRoute(
            methods: [ "GET" ],
            clusterName: cluster.Name,
            upstream: "/users/all",
            downstream: "/all");
        
        var clusters = new[] { cluster };
        var routes = new[] { route };
        
        // Act
        var result = _builder.Build(clusters, routes);
        var methods = result[0].Methods;

        // Assert
        Assert.Single(methods);
        Assert.Equal("GET", methods[0]);
    }
    
    [Fact]
    public void Build_WhenRouteHasMultipleMethods_ShouldBeMultiple()
    {
        // Arrange
        var cluster = CreateDummyCluster(
            name: "cluster1",
            prefix: "/cluster1",
            protocol: "http",
            hosts: [ "127.0.0.1" ],
            lbType: "rr");

        var route = CreateDummyRoute(
            methods: [ "GET", "POST" ],
            clusterName: cluster.Name,
            upstream: "/users/all",
            downstream: "/all");
        
        var clusters = new[] { cluster };
        var routes = new[] { route };
        
        // Act
        var result = _builder.Build(clusters, routes);
        var methods = result[0].Methods;

        // Assert
        Assert.Equal(2, methods.Length);
        Assert.Contains("GET", methods);
        Assert.Contains("POST", methods);
    }
    
    [Fact]
    public void Build_WhenUpstreamHasNoParameter_ShouldNotMarkAsParameter()
    {
        // Arrange
        var cluster = CreateDummyCluster(
            name: "cluster1",
            prefix: "/cluster1",
            protocol: "http",
            hosts: [ "127.0.0.1" ],
            lbType: "rr");
        
        var route = CreateDummyRoute(
            methods: [ "GET" ],
            clusterName: cluster.Name,
            upstream: "/users/id/info",
            downstream: "/info");
        
        var clusters = new[] { cluster };
        var routes = new[] { route };
        
        // Act
        var result = _builder.Build(clusters, routes);
        
        // Assert
        Assert.All(result[0].TotalUpstreamChunks, chunk => Assert.False(chunk.IsParameter));
    }
    
    [Fact]
    public void Build_WhenUpstreamHasSingleParameter_ShouldMarkAsParameter()
    {
        // Arrange
        var cluster = CreateDummyCluster(
            name: "cluster1",
            prefix: "/cluster1",
            protocol: "http",
            hosts: [ "127.0.0.1" ],
            lbType: "rr");
        
        var route = CreateDummyRoute(
            methods: [ "GET" ],
            clusterName: cluster.Name,
            upstream: "/users/{id}/info",
            downstream: "/info");
        
        var clusters = new[] { cluster };
        var routes = new[] { route };
        
        // Act
        var result = _builder.Build(clusters, routes);
        var chunk = result[0].TotalUpstreamChunks[1];
        
        // Assert
        Assert.True(chunk.IsParameter);
    }

    [Fact]
    public void Build_WhenUpstreamHasMultipleParameters_ShouldMarkAsParameter()
    {
        // Arrange
        var cluster = CreateDummyCluster(
            name: "cluster1",
            prefix: "/cluster1",
            protocol: "http",
            hosts: [ "127.0.0.1" ],
            lbType: "rr");
        
        var route = CreateDummyRoute(
            methods: [ "GET" ],
            clusterName: cluster.Name,
            upstream: "/users/{id}/info/{name}",
            downstream: "/info");
        
        var clusters = new[] { cluster };
        var routes = new[] { route };
        
        // Act
        var result = _builder.Build(clusters, routes);
        var chunks = result[0].TotalUpstreamChunks
            .Where(chunk => chunk.IsParameter);
        
        // Assert
        Assert.Equal(2, chunks.Count());
    }
    
    [Fact]
    public void Build_WhenDownstreamHasNoParameter_ShouldNotMarkAsParameter()
    {
        // Arrange
        var cluster = CreateDummyCluster(
            name: "cluster1",
            prefix: "/cluster1",
            protocol: "http",
            hosts: [ "127.0.0.1" ],
            lbType: "rr");
        
        var route = CreateDummyRoute(
            methods: [ "GET" ],
            clusterName: cluster.Name,
            upstream: "/users/id/info",
            downstream: "/id/info");
        
        var clusters = new[] { cluster };
        var routes = new[] { route };
        
        // Act
        var result = _builder.Build(clusters, routes);
        
        // Assert
        Assert.All(result[0].TotalDownstreamChunks, chunk => Assert.False(chunk.IsParameter));
    }
    
    [Fact]
    public void Build_WhenDownstreamHasSingleParameter_ShouldMarkAsParameter()
    {
        // Arrange
        var cluster = CreateDummyCluster(
            name: "cluster1",
            prefix: "/cluster1",
            protocol: "http",
            hosts: [ "127.0.0.1" ],
            lbType: "rr");
        
        var route = CreateDummyRoute(
            methods: [ "GET" ],
            clusterName: cluster.Name,
            upstream: "/users/{id}/info",
            downstream: "/{id}/info");
        
        var clusters = new[] { cluster };
        var routes = new[] { route };
        
        // Act
        var result = _builder.Build(clusters, routes);
        var chunk = result[0].TotalDownstreamChunks[0];
        
        // Assert
        Assert.True(chunk.IsParameter);
    }

    [Fact]
    public void Build_WhenDownstreamHasMultipleParameters_ShouldMarkAsParameter()
    {
        // Arrange
        var cluster = CreateDummyCluster(
            name: "cluster1",
            prefix: "/cluster1",
            protocol: "http",
            hosts: [ "127.0.0.1" ],
            lbType: "rr");
        
        var route = CreateDummyRoute(
            methods: [ "GET" ],
            clusterName: cluster.Name,
            upstream: "/users/{id}/info/{name}",
            downstream: "/{id}/info/{name}");
        
        var clusters = new[] { cluster };
        var routes = new[] { route };
        
        // Act
        var result = _builder.Build(clusters, routes);
        var chunks = result[0].TotalDownstreamChunks
            .Where(chunk => chunk.IsParameter);
        
        // Assert
        Assert.Equal(2, chunks.Count());
    }

    [Fact]
    public void Build_WhenParametersOrderIsSwapped_ShouldPreserveParamIndicesByName()
    {
        // Arrange
        var cluster = CreateDummyCluster(
            name: "cluster1",
            prefix: "/cluster1",
            protocol: "http",
            hosts: [ "127.0.0.1" ],
            lbType: "rr");
        
        var route = CreateDummyRoute(
            methods: [ "GET" ],
            clusterName: cluster.Name,
            upstream: "/x/{p1}/y/{p2}/z/{p3}",
            downstream: "/x/{p2}/y/{p3}/z/{p1}");
        
        var clusters = new[] { cluster };
        var routes = new[] { route };
        
        // Act
        var result = _builder.Build(clusters, routes);

        var downstreamParams = result[0].TotalDownstreamChunks
            .Where(chunk => chunk.IsParameter)
            .ToArray();
        
        // Assert
        Assert.Equal("{p2}", downstreamParams[0].Name);
        Assert.Equal("{p3}", downstreamParams[1].Name);
        Assert.Equal("{p1}", downstreamParams[2].Name);
    }
    
    [Fact]
    public void Build_WhenParametersOrderIsSwapped_ShouldBeValidParamIndexes()
    {
        // Arrange
        var cluster = CreateDummyCluster(
            name: "cluster1",
            prefix: "/cluster1",
            protocol: "http",
            hosts: [ "127.0.0.1" ],
            lbType: "rr");
        
        var route = CreateDummyRoute(
            methods: [ "GET" ],
            clusterName: cluster.Name,
            upstream: "/x/{p1}/y/{p2}/z/{p3}",
            downstream: "/x/{p2}/y/{p3}/z/{p1}");
        
        var clusters = new[] { cluster };
        var routes = new[] { route };
        
        // Act
        var result = _builder.Build(clusters, routes);

        var downstreamParams = result[0].TotalDownstreamChunks
            .Where(chunk => chunk.IsParameter)
            .ToArray();
        
        // Assert
        Assert.Equal(1, downstreamParams[0].ParamIndex);
        Assert.Equal(2, downstreamParams[1].ParamIndex);
        Assert.Equal(0, downstreamParams[2].ParamIndex);
    }

    [Fact]
    public void Build_Any_ShouldBeValidDownstreamSegmentsCount()
    {
        // Arrange
        var cluster = CreateDummyCluster(
            name: "cluster1",
            prefix: "/cluster1",
            protocol: "http",
            hosts: [ "127.0.0.1" ],
            lbType: "rr");

        var route = CreateDummyRoute(
            methods: [ "GET" ],
            clusterName: cluster.Name,
            upstream: "/users",
            downstream: "/users/all");
        
        var clusters = new[] { cluster };
        var routes = new[] { route };
        
        // Act
        var result = _builder.Build(clusters, routes);
        var downstreamSegments = result[0].DownstreamSegments;
        
        // Assert
        Assert.Equal(2, downstreamSegments.Length);
    }
    
    [Fact]
    public void Build_Any_ShouldBeValidDownstreamChunksCount()
    {
        // Arrange
        var cluster = CreateDummyCluster(
            name: "cluster1",
            prefix: "/cluster1",
            protocol: "http",
            hosts: [ "127.0.0.1" ],
            lbType: "rr");

        var route = CreateDummyRoute(
            methods: [ "GET" ],
            clusterName: cluster.Name,
            upstream: "/users",
            downstream: "/users/all");
        
        var clusters = new[] { cluster };
        var routes = new[] { route };
        
        // Act
        var result = _builder.Build(clusters, routes);
        var downstreamSegments = result[0].TotalDownstreamChunks;
        
        // Assert
        Assert.Equal(2, downstreamSegments.Count);
    }
    
    [Fact]
    public void Build_Any_ShouldBeValidDownstreamSegmentNames()
    {
        // Arrange
        var cluster = CreateDummyCluster(
            name: "cluster1",
            prefix: "/cluster1",
            protocol: "http",
            hosts: [ "127.0.0.1" ],
            lbType: "rr");

        var route = CreateDummyRoute(
            methods: [ "GET" ],
            clusterName: cluster.Name,
            upstream: "/users",
            downstream: "/users/all");
        
        var clusters = new[] { cluster };
        var routes = new[] { route };
        
        // Act
        var result = _builder.Build(clusters, routes);
        var downstreamSegments = result[0].DownstreamSegments;
        
        // Assert
        Assert.Equal("users", downstreamSegments[0]);
        Assert.Equal("all", downstreamSegments[1]);
    }
    
    [Fact]
    public void Build_Any_ShouldBeValidDownstreamChunksNames()
    {
        // Arrange
        var cluster = CreateDummyCluster(
            name: "cluster1",
            prefix: "/cluster1",
            protocol: "http",
            hosts: [ "127.0.0.1" ],
            lbType: "rr");

        var route = CreateDummyRoute(
            methods: [ "GET" ],
            clusterName: cluster.Name,
            upstream: "/users",
            downstream: "/users/all");
        
        var clusters = new[] { cluster };
        var routes = new[] { route };
        
        // Act
        var result = _builder.Build(clusters, routes);
        var downstreamSegments = result[0].TotalDownstreamChunks;
        
        // Assert
        Assert.Equal("users", downstreamSegments[0].Name);
        Assert.Equal("all", downstreamSegments[1].Name);
    }
}