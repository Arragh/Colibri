using Colibri.Configuration.Models;
using Colibri.Services.ConfigValidator;

namespace Unit.Tests.Validators;

public class CrossReferenceValidatorTests
{
    private readonly CrossReferenceValidator _validator = new();

    private readonly ClusterCfg[] _clusters =
    [
        new()
        {
            Name = "cluster1"
        },
        new()
        {
            Name = "cluster2"
        },
        new()
        {
            Name = "cluster3"
        }
    ];
    
    [Theory]
    [InlineData("cluster")]
    [InlineData("cluster5")]
    [InlineData("bombaster1")]
    public void ClusterExists_WhenClusterNotExists_ShouldReturnFalse(string name)
    {
        // Act
        var result = _validator.ClusterExists(name, _clusters);
        
        // Assert
        Assert.False(result);
    }
    
    [Theory]
    [InlineData("cluster1")]
    [InlineData("cluster2")]
    [InlineData("cluster3")]
    public void ClusterExists_WhenClusterExists_ShouldReturnTrue(string name)
    {
        // Act
        var result = _validator.ClusterExists(name, _clusters);
        
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void TotalUpstreamPathsLengthIsValid_WhenTotalUpstreamPathsLengthIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        ClusterCfg[] clusters =
        [
            new()
            {
                Prefix = "/path"
            }
        ];
        
        RouteCfg[] routes =
        [
            new()
            {
                UpstreamPattern = new string('x', 32767)
            },
            new()
            {
                UpstreamPattern = new string('x', 32767)
            }
        ];
        
        // Act
        var result = _validator.TotalUpstreamPathsLengthIsValid(clusters, routes);
        
        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void TotalUpstreamPathsLengthIsValid_WhenTotalUpstreamPathsLengthIsValid_ShouldReturnTrue()
    {
        // Arrange
        ClusterCfg[] clusters =
        [
            new()
            {
                Prefix = "/path"
            }
        ];
        
        RouteCfg[] routes =
        [
            new()
            {
                UpstreamPattern = new string('x', 32764)
            },
            new()
            {
                UpstreamPattern = new string('x', 32764)
            }
        ];
        
        // Act
        var result = _validator.TotalUpstreamPathsLengthIsValid(clusters, routes);
        
        // Assert
        Assert.True(result);
    }
}