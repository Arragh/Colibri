using Colibri.Helpers;
using Colibri.Snapshots.RoutingSnapshot;
using Tests.Helpers;

namespace Tests.Tests.RoutingEngine;

public class RoutingEngineTests
{
    [Fact]
    public void TryMatch_InvalidPath_ResultIsFalse_ReturnsNull()
    {
        // Arrange
        var routingSnapshotBuilder = new RoutingSnapshotBuilder();
        var settings = RoutingSettingsHelper.MultipleClusters_MultipleRoutes();
        
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
    public void TryMatch_ValidPath_ResultIsTrue_ReturnsNotNull()
    {
        // Arrange
        var routingSnapshotBuilder = new RoutingSnapshotBuilder();
        var settings = RoutingSettingsHelper.MultipleClusters_MultipleRoutes();
        
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
}