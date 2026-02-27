using Colibri.Configuration.Models;
using Colibri.Runtime.Snapshots.Cluster;

namespace Unit.Tests.Snapshots;

public sealed class ClusterSnapshotBuilderTests
{
    private readonly ClusterSnapshotBuilder _clusterSnapshotBuilder = new();

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
    
    [Fact]
    public void Build_Any_ShouldBeNotNull()
    { 
        // Arrange
        var cluster = CreateDummyCluster(
            name: "cluster1",
            prefix: "/cluster1",
            protocol: "http",
            hosts: [ "127.0.0.1" ],
            lbType: "rr");
        
        var clusters = new [] { cluster };
        
        // Act
        var result = _clusterSnapshotBuilder.Build(clusters);
        
        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void Build_WhenNoClusters_ShouldBeEmpty()
    {
        // Arrange
        var clusters = new ClusterCfg[] { };

        // Act
        var result = _clusterSnapshotBuilder.Build(clusters);
        
        // Assert
        Assert.Empty(result.Clusters);
    }
    
    [Fact]
    public void Build_WhenClusterDisabled_ShouldBeEmpty()
    {
        // Arrange
        var cluster = CreateDummyCluster(
            name: "cluster1",
            prefix: "/cluster1",
            protocol: "http",
            hosts: [ "127.0.0.1" ],
            lbType: "rr");
        
        cluster.Enabled = false;
        var clusters = new [] { cluster };
        
        // Act
        var result = _clusterSnapshotBuilder.Build(clusters);
        
        // Assert
        Assert.Empty(result.Clusters);
    }

    [Fact]
    public void Build_WhenSingleClusterEnabled_ShouldIncludeSingle()
    {
        // Arrange
        var cluster1 = CreateDummyCluster(
            name: "cluster1",
            prefix: "/cluster1",
            protocol: "http",
            hosts: [ "127.0.0.1" ],
            lbType: "rr");
        
        var cluster2 = CreateDummyCluster(
            name: "cluster2",
            prefix: "/cluster2",
            protocol: "http",
            hosts: [ "127.0.0.1" ],
            lbType: "rr");
        
        cluster1.Enabled = true;
        cluster2.Enabled = false;
        
        var clusters = new [] { cluster1, cluster2 };
        
        // Act
        var result = _clusterSnapshotBuilder.Build(clusters);
        
        // Assert
        Assert.Single(result.Clusters);
    }

    [Fact]
    public void Build_WhenMultipleClusterEnabled_ShouldIncludeMultiple()
    {
        // Arrange
        var cluster1 = CreateDummyCluster(
            name: "cluster1",
            prefix: "/cluster1",
            protocol: "http",
            hosts: [ "127.0.0.1" ],
            lbType: "rr");
        
        var cluster2 = CreateDummyCluster(
            name: "cluster2",
            prefix: "/cluster2",
            protocol: "http",
            hosts: [ "127.0.0.1" ],
            lbType: "rr");
        
        cluster1.Enabled = true;
        cluster2.Enabled = true;
        
        var clusters = new [] { cluster1, cluster2 };
        
        // Act
        var result = _clusterSnapshotBuilder.Build(clusters);
        
        // Assert
        Assert.Equal(2, result.Clusters.Length);
    }
}