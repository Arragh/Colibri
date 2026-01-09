using Colibri.Configuration;
using Colibri.Configuration.Models;

namespace Tests.Tests.Validator;

public class ConfigValidatorTests
{
    [Fact]
    public void AllSettingsIsNull_ResultIsFalse()
    {
        // Arrange
        var settings = new ColibriSettings();
        
        // Act
        var validateResult = ConfigValidator.Validate(settings);
        
        //Assert
        Assert.False(validateResult);
    }

    [Fact]
    public void ClustersIsEmpty_ResultIsFalse()
    {
        var settings = new ColibriSettings
        {
            Routing = new RoutingSettings
            {
                Clusters = []
            }
        };
        
        // Act
        var validateResult = ConfigValidator.Validate(settings);
        
        //Assert
        Assert.False(validateResult);
    }

    [Fact]
    public void SingleEmptyCluster_ResultIsFalse()
    {
        var settings = new ColibriSettings
        {
            Routing = new RoutingSettings
            {
                Clusters =
                [
                    new ClusterDto()
                ]
            }
        };
        
        // Act
        var validateResult = ConfigValidator.Validate(settings);
        
        //Assert
        Assert.False(validateResult);
    }

    [Fact]
    public void SingleCluster_AllFieldsAreEmpty_ResultIsFalse()
    {
        var settings = new ColibriSettings
        {
            Routing = new RoutingSettings
            {
                Clusters = [
                    new ClusterDto
                    {
                        Protocol = string.Empty,
                        Hosts = [],
                        Routes = []
                    }
                ]
            }
        };
        
        // Act
        var validateResult = ConfigValidator.Validate(settings);
        
        //Assert
        Assert.False(validateResult);
    }
    
    [Fact]
    public void SingleCluster_EmptyHosts_EmptyRoutes_ResultIsFalse()
    {
        var settings = new ColibriSettings
        {
            Routing = new RoutingSettings
            {
                Clusters = [
                    new ClusterDto
                    {
                        Protocol = "Http",
                        Hosts = [],
                        Routes = []
                    }
                ]
            }
        };
        
        // Act
        var validateResult = ConfigValidator.Validate(settings);
        
        //Assert
        Assert.False(validateResult);
    }
    
    [Fact]
    public void SingleCluster_EmptyRoutes_ResultIsFalse()
    {
        var settings = new ColibriSettings
        {
            Routing = new RoutingSettings
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
            }
        };
        
        // Act
        var validateResult = ConfigValidator.Validate(settings);
        
        //Assert
        Assert.False(validateResult);
    }
    
    [Fact]
    public void SingleCluster_SingleBlankRoute_ResultIsFalse()
    {
        var settings = new ColibriSettings
        {
            Routing = new RoutingSettings
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
            }
        };
        
        // Act
        var validateResult = ConfigValidator.Validate(settings);
        
        //Assert
        Assert.False(validateResult);
    }
    
    [Fact]
    public void SingleCluster_SingleRoute_AllFieldsAreEmpty_ResultIsFalse()
    {
        var settings = new ColibriSettings
        {
            Routing = new RoutingSettings
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
            }
        };
        
        // Act
        var validateResult = ConfigValidator.Validate(settings);
        
        //Assert
        Assert.False(validateResult);
    }
    
    [Fact]
    public void SingleCluster_SingleRoute_EmptyUpstream_EmptyDownstream_ResultIsFalse()
    {
        var settings = new ColibriSettings
        {
            Routing = new RoutingSettings
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
            }
        };
        
        // Act
        var validateResult = ConfigValidator.Validate(settings);
        
        //Assert
        Assert.False(validateResult);
    }
    
    [Fact]
    public void SingleCluster_SingleRoute_EmptyDownstream_ResultIsFalse()
    {
        var settings = new ColibriSettings
        {
            Routing = new RoutingSettings
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
            }
        };
        
        // Act
        var validateResult = ConfigValidator.Validate(settings);
        
        //Assert
        Assert.False(validateResult);
    }
    
    [Fact]
    public void SingleCluster_SingleRoute_ResultIsTrue()
    {
        var settings = new ColibriSettings
        {
            Routing = new RoutingSettings
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
            }
        };
        
        // Act
        var validateResult = ConfigValidator.Validate(settings);
        
        //Assert
        Assert.True(validateResult);
    }
    
    [Fact]
    public void SingleCluster_EmptyAuthorize_ResultIsTrue()
    {
        var settings = new ColibriSettings
        {
            Routing = new RoutingSettings
            {
                Clusters = [
                    new ClusterDto
                    {
                        Authorize = new AuthorizeDto(),
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
            }
        };
        
        // Act
        var validateResult = ConfigValidator.Validate(settings);
        
        //Assert
        Assert.True(validateResult);
    }
    
    [Fact]
    public void SingleCluster_AuthorizeIsNotRequired_AllFieldsAreEmpty_ResultIsTrue()
    {
        var settings = new ColibriSettings
        {
            Routing = new RoutingSettings
            {
                Clusters = [
                    new ClusterDto
                    {
                        Authorize = new AuthorizeDto
                        {
                            Required = false,
                            Policy = string.Empty,
                            Roles = []
                        },
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
            }
        };
        
        // Act
        var validateResult = ConfigValidator.Validate(settings);
        
        //Assert
        Assert.True(validateResult);
    }
    
    [Fact]
    public void SingleCluster_AuthorizeIsNotRequired_ResultIsTrue()
    {
        var settings = new ColibriSettings
        {
            Routing = new RoutingSettings
            {
                Clusters = [
                    new ClusterDto
                    {
                        Authorize = new AuthorizeDto
                        {
                            Required = false,
                            Policy = "Policy",
                            Roles = []
                        },
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
            }
        };
        
        // Act
        var validateResult = ConfigValidator.Validate(settings);
        
        //Assert
        Assert.True(validateResult);
    }
    
    [Fact]
    public void AuthorizationIsNull_SingleCluster_AuthorizeIsRequired_ResultIsFalse()
    {
        var settings = new ColibriSettings
        {
            Routing = new RoutingSettings
            {
                Clusters = [
                    new ClusterDto
                    {
                        Authorize = new AuthorizeDto
                        {
                            Required = true,
                            Policy = "Policy",
                            Roles = []
                        },
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
            }
        };
        
        // Act
        var validateResult = ConfigValidator.Validate(settings);
        
        //Assert
        Assert.False(validateResult);
    }
    
    [Fact]
    public void AuthorizationIsEmpty_SingleCluster_AuthorizeIsRequired_ResultIsFalse()
    {
        var settings = new ColibriSettings
        {
            Authorization = new AuthorizationSettings(),
            Routing = new RoutingSettings
            {
                Clusters = [
                    new ClusterDto
                    {
                        Authorize = new AuthorizeDto
                        {
                            Required = true,
                            Policy = "Policy",
                            Roles = []
                        },
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
            }
        };
        
        // Act
        var validateResult = ConfigValidator.Validate(settings);
        
        //Assert
        Assert.False(validateResult);
    }
    
    [Fact]
    public void Authorization_PoliciesAreEmpty_AuthorizeIsRequired_ResultIsFalse()
    {
        var settings = new ColibriSettings
        {
            Authorization = new AuthorizationSettings
            {
                Policies = []
            },
            Routing = new RoutingSettings
            {
                Clusters = [
                    new ClusterDto
                    {
                        Authorize = new AuthorizeDto
                        {
                            Required = true,
                            Policy = "Policy",
                            Roles = []
                        },
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
            }
        };
        
        // Act
        var validateResult = ConfigValidator.Validate(settings);
        
        //Assert
        Assert.False(validateResult);
    }
    
    [Fact]
    public void Authorization_SingleEmptyPolicy_AuthorizeIsRequired_ResultIsFalse()
    {
        var settings = new ColibriSettings
        {
            Authorization = new AuthorizationSettings
            {
                Policies = [
                    new PolicyDto()
                ]
            },
            Routing = new RoutingSettings
            {
                Clusters = [
                    new ClusterDto
                    {
                        Authorize = new AuthorizeDto
                        {
                            Required = true,
                            Policy = "Policy",
                            Roles = []
                        },
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
            }
        };
        
        // Act
        var validateResult = ConfigValidator.Validate(settings);
        
        //Assert
        Assert.False(validateResult);
    }
    
    [Fact]
    public void Authorization_SinglePolicy_AllFieldsAreEmpty_AuthorizeIsRequired_ResultIsFalse()
    {
        var settings = new ColibriSettings
        {
            Authorization = new AuthorizationSettings
            {
                Policies = [
                    new PolicyDto
                    {
                        Name = string.Empty,
                        PublicKey = string.Empty,
                    }
                ]
            },
            Routing = new RoutingSettings
            {
                Clusters = [
                    new ClusterDto
                    {
                        Authorize = new AuthorizeDto
                        {
                            Required = true,
                            Policy = "Policy",
                            Roles = []
                        },
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
            }
        };
        
        // Act
        var validateResult = ConfigValidator.Validate(settings);
        
        //Assert
        Assert.False(validateResult);
    }
    
    [Fact]
    public void Authorization_SinglePolicy_PublicKeyIsEmpty_AuthorizeIsRequired_ResultIsFalse()
    {
        var settings = new ColibriSettings
        {
            Authorization = new AuthorizationSettings
            {
                Policies = [
                    new PolicyDto
                    {
                        Name = "Policy",
                        PublicKey = string.Empty,
                    }
                ]
            },
            Routing = new RoutingSettings
            {
                Clusters = [
                    new ClusterDto
                    {
                        Authorize = new AuthorizeDto
                        {
                            Required = true,
                            Policy = "Policy",
                            Roles = []
                        },
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
            }
        };
        
        // Act
        var validateResult = ConfigValidator.Validate(settings);
        
        //Assert
        Assert.False(validateResult);
    }
    
    [Fact]
    public void Authorization_SinglePolicy_AuthorizeIsRequired_ResultIsTrue()
    {
        var settings = new ColibriSettings
        {
            Authorization = new AuthorizationSettings
            {
                Policies = [
                    new PolicyDto
                    {
                        Name = "Policy",
                        PublicKey = "secret-key",
                    }
                ]
            },
            Routing = new RoutingSettings
            {
                Clusters = [
                    new ClusterDto
                    {
                        Authorize = new AuthorizeDto
                        {
                            Required = true,
                            Policy = "Policy",
                            Roles = []
                        },
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
            }
        };
        
        // Act
        var validateResult = ConfigValidator.Validate(settings);
        
        //Assert
        Assert.True(validateResult);
    }
}