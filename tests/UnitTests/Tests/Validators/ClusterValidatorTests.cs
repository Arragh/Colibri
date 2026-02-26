using Colibri.Configuration.Models;
using Colibri.Helpers;
using Colibri.Services.ConfigValidator;

namespace UnitTests.Tests.Validators;

public sealed class ClusterValidatorTests
{
    private readonly ClusterValidator _validator = new();
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void NameIsNotEmpty_WhenNameIsEmpty_ShouldReturnFalse(string name)
    {
        // Act
        var result = _validator.NameIsNotEmpty(name);
        
        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void NameLengthIsValid_WhenNameLengthIsMoreThenMax_ShouldReturnFalse()
    {
        // Arrange
        var name = string.Empty;
        for (int i = 0; i < GlobalConstants.SegmentMaxLength + 1; i++)
        {
            name += 'x';
        }
        
        // Act
        var result = _validator.NameLengthIsValid(name);
        
        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void NameLengthIsValid_WhenNameLengthIsLessOrMax_ShouldReturnTrue()
    {
        // Arrange
        var name = string.Empty;
        for (int i = 0; i < GlobalConstants.SegmentMaxLength; i++)
        {
            name += 'x';
        }
        
        // Act
        var result = _validator.NameLengthIsValid(name);
        
        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("cluster 1")]
    [InlineData("cluster-1")]
    [InlineData("cluster_1")]
    [InlineData("cluster.1")]
    public void NameIsValid_WhenNameIsInvalid_ShouldReturnFalse(string name)
    {
        // Act
        var result = _validator.NameIsValid(name);
        
        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void NameIsValid_WhenNameIsValid_ShouldReturnTrue()
    {
        // Arrange
        string name = "cluster1";
        
        // Act
        var result = _validator.NameIsValid(name);
        
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void NameIsUnique_WhenNameIsNotUnique_ShouldReturnFalse()
    {
        // Arrange
        var clusters = new ClusterCfg[]
        {
            new ClusterCfg
            {
                Name = "cluster1"
            },
            new ClusterCfg
            {
                Name = "cluster1"
            }
        };
        
        // Act
        var result = _validator.NameIsUnique(clusters[0], clusters);
        
        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void NameIsUnique_WhenNameIsUnique_ShouldReturnTrue()
    {
        // Arrange
        var clusters = new ClusterCfg[]
        {
            new ClusterCfg
            {
                Name = "cluster1"
            },
            new ClusterCfg
            {
                Name = "cluster2"
            }
        };
        
        // Act
        var result = _validator.NameIsUnique(clusters[0], clusters);
        
        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void ProtocolIsNotEmpty_WhenProtocolIsEmpty_ShouldReturnFalse(string protocol)
    {
        // Act
        var result = _validator.ProtocolIsNotEmpty(protocol);
        
        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void ProtocolIsNotEmpty_WhenProtocolIsNotEmpty_ShouldReturnTrue()
    {
        // Arange
        var protocol = "http";
        
        // Act
        var result = _validator.ProtocolIsNotEmpty(protocol);
        
        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("grpc")]
    [InlineData("tcp/ip")]
    [InlineData("trololo")]
    public void ProtocolIsValid_WhenProtocolIsInvalid_ShouldReturnFalse(string protocol)
    {
        // Act
        var result = _validator.ProtocolIsValid(protocol);
        
        // Assert
        Assert.False(result);
    }
    
    [Theory]
    [InlineData("http")]
    [InlineData("ws")]
    public void ProtocolIsValid_WhenProtocolIsValid_ShouldReturnTrue(string protocol)
    {
        // Act
        var result = _validator.ProtocolIsValid(protocol);
        
        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void PrefixIsNotEmpty_WhenPrefixIsEmpty_ShouldReturnFalse(string prefix)
    {
        // Act
        var result = _validator.PrefixIsNotEmpty(prefix);
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void PrefixIsNotEmpty_WhenPrefixIsNotEmpty_ShouldReturnTrue()
    {
        // Arrange
        var prefix = "/cluster1";
        
        // Act
        var result = _validator.PrefixIsNotEmpty(prefix);
        
        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("cluster1")]
    [InlineData("cluster-1")]
    [InlineData("cluster_1")]
    public void PrefixIsValid_WhenPrefixIsInvalid_ShouldReturnFalse(string prefix)
    {
        // Act
        var result = _validator.PrefixIsValid(prefix);
        
        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("/cluster1")]
    [InlineData("/cluster-1")]
    [InlineData("/cluster_1")]
    public void PrefixIsValid_WhenPrefixIsValid_ShouldReturnTrue(string prefix)
    {
        var result = _validator.PrefixIsValid(prefix);
        
        Assert.True(result);
    }

    [Fact]
    public void HostsAreNotEmpty_WhenHostsAreEmpty_ShouldReturnFalse()
    {
        // Arrange
        var hosts = Array.Empty<string>();
        
        // Act
        var result = _validator.HostsAreNotEmpty(hosts);
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HostsAreNotEmpty_WhenHostsAreNotEmpty_ShouldReturnTrue()
    {
        // Arrange
        var hosts = new[] { "127.0.0.1:5000", "localhost:8080" };
        
        // Act
        var result = _validator.HostsAreNotEmpty(hosts);
        
        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void LoadBalancerTypeIsNotEmpty_WhenLoadBalancerTypeIsEmpty_ShouldReturnFalse(string type)
    {
        // Act
        var result = _validator.LoadBalancerTypeIsNotEmpty(type);
        
        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void LoadBalancerTypeIsNotEmpty_WhenLoadBalancerTypeNotEmpty_ShouldReturnTrue()
    {
        // Arrange
        var type = "rr";
        
        // Act
        var result = _validator.LoadBalancerTypeIsNotEmpty(type);
        
        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("round-robin")]
    [InlineData("random")]
    [InlineData("some-unexpected-type")]
    public void LoadBalancerTypeIsValid_WhenLoadBalancerTypeIsInvalid_ShouldReturnFalse(string type)
    {
        // Act
        var result = _validator.LoadBalancerTypeIsValid(type);
        
        // Assert
        Assert.False(result);
    }
    
    [Theory]
    [InlineData("rr")]
    [InlineData("rnd")]
    public void LoadBalancerTypeIsValid_WhenLoadBalancerTypeIsValid_ShouldReturnTrue(string type)
    {
        // Act
        var result = _validator.LoadBalancerTypeIsValid(type);
        
        // Assert
        Assert.True(result);
    }
}