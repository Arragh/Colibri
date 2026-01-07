using Colibri.Configuration;
using Colibri.Configuration.Models;

namespace Tests;

public class ConfigValidatorTests
{
    [Fact]
    public void NullClusters_ResultFalse()
    {
        // Arrange
        var settings = new RoutingSettings();
        
        // Act
        var validateResult = ConfigValidator.Validate(settings);
        
        //Assert
        Assert.False(validateResult);
    }

    [Fact]
    public void EmptyClusters_ResultFalse()
    {
        var settings = new RoutingSettings
        {
            Clusters = []
        };
        
        // Act
        var validateResult = ConfigValidator.Validate(settings);
        
        //Assert
        Assert.False(validateResult);
    }

    [Fact]
    public void SingleBlankCluster_ResultFalse()
    {
        var settings = new RoutingSettings
        {
            Clusters = [
                new ClusterDto()
            ]
        };
        
        // Act
        var validateResult = ConfigValidator.Validate(settings);
        
        //Assert
        Assert.False(validateResult);
    }

    [Fact]
    public void SingleCluster_EmptyProtocol_EmptyHosts_EmptyRoutes_ResultFalse()
    {
        var settings = new RoutingSettings
        {
            Clusters = [
                new ClusterDto
                {
                    Protocol = string.Empty,
                    Hosts = [],
                    Routes = []
                }
            ]
        };
        
        // Act
        var validateResult = ConfigValidator.Validate(settings);
        
        //Assert
        Assert.False(validateResult);
    }
    
    [Fact]
    public void SingleCluster_EmptyHosts_EmptyRoutes_ResultFalse()
    {
        var settings = new RoutingSettings
        {
            Clusters = [
                new ClusterDto
                {
                    Protocol = "Http",
                    Hosts = [],
                    Routes = []
                }
            ]
        };
        
        // Act
        var validateResult = ConfigValidator.Validate(settings);
        
        //Assert
        Assert.False(validateResult);
    }
    
    [Fact]
    public void SingleCluster_EmptyRoutes_ResultFalse()
    {
        var settings = new RoutingSettings
        {
            Clusters = [
                new ClusterDto
                {
                    Protocol = "Http",
                    Hosts = [
                        "http://trololo.com"
                    ],
                    Routes = []
                }
            ]
        };
        
        // Act
        var validateResult = ConfigValidator.Validate(settings);
        
        //Assert
        Assert.False(validateResult);
    }
    
    [Fact]
    public void SingleCluster_SingleBlankRoute_ResultFalse()
    {
        var settings = new RoutingSettings
        {
            Clusters = [
                new ClusterDto
                {
                    Protocol = "Http",
                    Hosts = [
                        "http://trololo.com"
                    ],
                    Routes = [
                        new RouteDto()
                    ]
                }
            ]
        };
        
        // Act
        var validateResult = ConfigValidator.Validate(settings);
        
        //Assert
        Assert.False(validateResult);
    }
    
    [Fact]
    public void SingleCluster_SingleRoute_EmptyMethod_EmptyUpstream_EmptyDownstream_ResultFalse()
    {
        var settings = new RoutingSettings
        {
            Clusters = [
                new ClusterDto
                {
                    Protocol = "Http",
                    Hosts = [
                        "http://trololo.com"
                    ],
                    Routes = [
                        new RouteDto
                        {
                            Method = string.Empty,
                            UpstreamPattern = string.Empty,
                            DownstreamPattern = string.Empty
                        }
                    ]
                }
            ]
        };
        
        // Act
        var validateResult = ConfigValidator.Validate(settings);
        
        //Assert
        Assert.False(validateResult);
    }
    
    [Fact]
    public void SingleCluster_SingleRoute_EmptyUpstream_EmptyDownstream_ResultFalse()
    {
        var settings = new RoutingSettings
        {
            Clusters = [
                new ClusterDto
                {
                    Protocol = "Http",
                    Hosts = [
                        "http://trololo.com"
                    ],
                    Routes = [
                        new RouteDto
                        {
                            Method = "GET",
                            UpstreamPattern = string.Empty,
                            DownstreamPattern = string.Empty
                        }
                    ]
                }
            ]
        };
        
        // Act
        var validateResult = ConfigValidator.Validate(settings);
        
        //Assert
        Assert.False(validateResult);
    }
    
    [Fact]
    public void SingleCluster_SingleRoute_EmptyDownstream_ResultFalse()
    {
        var settings = new RoutingSettings
        {
            Clusters = [
                new ClusterDto
                {
                    Protocol = "Http",
                    Hosts = [
                        "http://trololo.com"
                    ],
                    Routes = [
                        new RouteDto
                        {
                            Method = "GET",
                            UpstreamPattern = "/test1/get",
                            DownstreamPattern = string.Empty
                        }
                    ]
                }
            ]
        };
        
        // Act
        var validateResult = ConfigValidator.Validate(settings);
        
        //Assert
        Assert.False(validateResult);
    }
    
    [Fact]
    public void SingleCluster_SingleRoute_ResultTrue()
    {
        var settings = new RoutingSettings
        {
            Clusters = [
                new ClusterDto
                {
                    Protocol = "Http",
                    Hosts = [
                        "http://trololo.com"
                    ],
                    Routes = [
                        new RouteDto
                        {
                            Method = "GET",
                            UpstreamPattern = "/test1/get",
                            DownstreamPattern = "/get"
                        }
                    ]
                }
            ]
        };
        
        // Act
        var validateResult = ConfigValidator.Validate(settings);
        
        //Assert
        Assert.True(validateResult);
    }
}