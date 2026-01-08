using Colibri.Configuration;
using Colibri.Configuration.Models;
using Colibri.Helpers;
using Colibri.Snapshots.RoutingSnapshot;

namespace Tests.Tests.RoutingEngine;

/*
 * Так как в данных тестах проверяется точное совпадение маршрутов и методов,
 * то вместо общего глобального конфига используется локальный.
 */
public class RoutingEngineTests
{
    [Fact]
    public void TryMatch_PathIsInvalid_ResultIsFalse_ReturnsNull()
    {
        // Arrange
        var routingSnapshotBuilder = new RoutingSnapshotBuilder();
        var settings = GetLocalSettings();
        
        // Act
        var routingSnapshot = routingSnapshotBuilder.Build(settings);
        var falseRequestPath = "/ololo/trololo".AsSpan();
        var methodMask = HttpMethodMask.GetMask("GET");
        var routingEngine = new Colibri.Services.RoutingEngine.RoutingEngine();
        
        // Assert
        var matchResult = routingEngine.TryMatch(
            routingSnapshot,
            falseRequestPath,
            methodMask,
            out var downstream);
        
        Assert.False(matchResult);
        Assert.Null(downstream);
    }
    
    [Fact]
    public void TryMatch_PathIsValid_MethodIsInvalid_ResultIsFalse_ReturnsNull()
    {
        // Arrange
        var routingSnapshotBuilder = new RoutingSnapshotBuilder();
        var settings = GetLocalSettings();
        
        // Act
        var routingSnapshot = routingSnapshotBuilder.Build(settings);
        var routingEngine = new Colibri.Services.RoutingEngine.RoutingEngine();
        
        var requestPath = "/tests/users".AsSpan();
        var methodMask = HttpMethodMask.GetMask("GET");
        
        var matchResult = routingEngine.TryMatch(
            routingSnapshot,
            requestPath,
            methodMask,
            out var downstream);
        
        // Assert
        Assert.False(matchResult);
        Assert.Null(downstream);
    }
    
    [Fact]
    public void TryMatch_PathIsValid_ResultIsTrue_ReturnsNotNull()
    {
        // Arrange
        var routingSnapshotBuilder = new RoutingSnapshotBuilder();
        var settings = GetLocalSettings();
        
        // Act
        var routingSnapshot = routingSnapshotBuilder.Build(settings);
        var routingEngine = new Colibri.Services.RoutingEngine.RoutingEngine();
        
        var requestPath1 = "/tests/users".AsSpan();
        var methodMask1 = HttpMethodMask.GetMask("POST");
        
        var requestPath2 = "/tests/users/me".AsSpan();
        var methodMask2 = HttpMethodMask.GetMask("GET");
        
        var matchResult1 = routingEngine.TryMatch(
            routingSnapshot,
            requestPath1,
            methodMask1,
            out var downstream1);
        
        var matchResult2 = routingEngine.TryMatch(
            routingSnapshot,
            requestPath2,
            methodMask2,
            out var downstream2);
        
        // Assert
        Assert.True(matchResult1);
        Assert.True(matchResult2);
        Assert.NotNull(downstream1);
        Assert.NotNull(downstream2);
    }
    
    private RoutingSettings GetLocalSettings()
    {
        return new RoutingSettings
        {
            Clusters = [
                new ClusterDto
                {
                    Protocol = "Http",
                    Hosts = [
                        "http://escapepod:5000",
                        "http://escapepod:5001"
                    ],
                    Routes =
                    [
                        new RouteDto
                        {
                            Method = "POST",
                            UpstreamPattern = "/tests/users",
                            DownstreamPattern = "/internal/users"
                        },
                        new RouteDto
                        {
                            Method = "GET",
                            UpstreamPattern = "/tests/users/{name}/info",
                            DownstreamPattern = "/profile/{login}/info"
                        },
                        new RouteDto
                        {
                            Method = "GET",
                            UpstreamPattern = "/tests/users/me",
                            DownstreamPattern = "/api/clients"
                        }
                    ]
                },
            ]
        };
    }
}