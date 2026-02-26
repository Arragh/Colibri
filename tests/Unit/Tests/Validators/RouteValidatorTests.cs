using Colibri.Configuration.Models;
using Colibri.Helpers;
using Colibri.Services.ConfigValidator;

namespace Unit.Tests.Validators;

public sealed class RouteValidatorTests
{
    private readonly RouteValidator _validator = new();

    public static readonly TheoryData<string> ValidPatterns = new()
    {
        "/",
        "/{id}",
        "/{id}/{next}",
        "/{id-next}",
        "/{id_next}",
        "/users",
        "/users/{id}",
        "/users/{id}/{next}",
        "/users/{id}/info",
        "/users/{id}/info/{next}",
        "/users-list",
        "/users-list/all",
        "/users_list",
        "/users_list/all",
        "/users/{id-next}",
        "/users/{id_next}",
        "/users/{id-next}/info",
        "/users/{id-next}/info/{name}",
        "/users/{id_next}/info",
        "/users/{id_next}/info/{name}/{next}"
    };
    
    [Theory]
    [InlineData("")]
    [InlineData("users")]
    [InlineData("/users/")]
    [InlineData("/users/{id}/")]
    public void PatternFormatIsValid_WhenPatternIsInvalid_ShouldReturnFalse(string pattern)
    {
        // Act
        var result = _validator.PatternFormatIsValid(pattern);
        
        // Assert
        Assert.False(result);
    }

    [Theory]
    [MemberData(nameof(ValidPatterns))]
    public void PatternFormatIsValid_WhenPatternIsValid_ShouldReturnTrue(string pattern)
    {
        // Act
        var result = _validator.PatternFormatIsValid(pattern);
        
        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("//")]
    [InlineData("/users//")]
    [InlineData("/users//{id}")]
    public void PatternSegmentsNotEmpty_WhenPatternHasEmptySegments_ShouldReturnFalse(string pattern)
    {
        // Act
        var result = _validator.PatternSegmentsNotEmpty(pattern);
        
        // Assert
        Assert.False(result);
    }
    
    [Theory]
    [MemberData(nameof(ValidPatterns))]
    public void PatternSegmentsNotEmpty_WhenPatternHasNoEmptySegments_ShouldReturnTrue(string pattern)
    {
        // Act
        var result = _validator.PatternSegmentsNotEmpty(pattern);
        
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void PatternStaticSegmentsLengthIsValid_WhenPatternStaticSegmentsLengthIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var pattern = string.Empty;
        for (int i = 0; i < GlobalConstants.SegmentMaxLength + 1; i++)
        {
            pattern += 'x';
        }
        
        // Act
        var result = _validator.PatternStaticSegmentsLengthIsValid(pattern);
        
        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void PatternStaticSegmentsLengthIsValid_WhenPatternStaticSegmentsLengthIsValid_ShouldReturnTrue()
    {
        // Arrange
        var pattern = string.Empty;
        for (int i = 0; i < GlobalConstants.SegmentMaxLength; i++)
        {
            pattern += 'x';
        }
        
        // Act
        var result = _validator.PatternStaticSegmentsLengthIsValid(pattern);
        
        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("/+users")]
    [InlineData("/users*pattern")]
    [InlineData("/users?pattern")]
    public void PatternStaticSegmentNamesIsValid_WhenPatternStaticSegmentNamesIsInvalid_ShouldReturnFalse(string pattern)
    {
        // Act
        var result = _validator.PatternStaticSegmentNamesIsValid(pattern);
        
        // Assert
        Assert.False(result);
    }
    
    [Theory]
    [MemberData(nameof(ValidPatterns))]
    public void PatternStaticSegmentNamesIsValid_WhenPatternStaticSegmentNamesIsValid_ShouldReturnTrue(string pattern)
    {
        // Act
        var result = _validator.PatternStaticSegmentNamesIsValid(pattern);
        
        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("/users/{name")]
    [InlineData("/users/name}")]
    [InlineData("/users/id}/info")]
    [InlineData("/users/{id/info")]
    public void PatternParamsCurlyBracesIsValid_WhenPatternParamsCurlyBracesIsInvalid_ShouldReturnFalse(string pattern)
    {
        // Act
        var result = _validator.PatternParamsCurlyBracesIsValid(pattern);
        
        // Assert
        Assert.False(result);
    }
    
    [Theory]
    [MemberData(nameof(ValidPatterns))]
    public void PatternParamsCurlyBracesIsValid_WhenPatternParamsCurlyBracesIsValid_ShouldReturnTrue(string pattern)
    {
        // Act
        var result = _validator.PatternParamsCurlyBracesIsValid(pattern);
        
        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("/{}")]
    [InlineData("/users/{}")]
    [InlineData("/users/{}/info")]
    public void PatternParamNamesNotEmpty_WhenPatternParamNamesIsEmpty_ShouldReturnFalse(string pattern)
    {
        // Act
        var result = _validator.PatternParamNamesNotEmpty(pattern);
        
        // Assert
        Assert.False(result);
    }
    
    [Theory]
    [MemberData(nameof(ValidPatterns))]
    public void PatternParamNamesNotEmpty_WhenPatternParamNamesNotEmpty_ShouldReturnTrue(string pattern)
    {
        // Act
        var result = _validator.PatternParamNamesNotEmpty(pattern);
        
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void PatternParamsLengthIsValid_WhenPatternParamsLengthIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var pattern = "{";
        for (int i = 0; i < GlobalConstants.SegmentMaxLength - 1; i++)
        {
            pattern += 'x';
        }
        pattern += '}';
        
        // Act
        var result = _validator.PatternParamsLengthIsValid(pattern);
        
        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void PatternParamsLengthIsValid_WhenPatternParamsLengthIsValid_ShouldReturnTrue()
    {
        // Arrange
        var pattern = "{";
        for (int i = 0; i < GlobalConstants.SegmentMaxLength - 2; i++)
        {
            pattern += 'x';
        }
        pattern += '}';
        
        // Act
        var result = _validator.PatternParamsLengthIsValid(pattern);
        
        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("/{+users}")]
    [InlineData("/users/{*pattern}")]
    [InlineData("/users/{?pattern}")]
    [InlineData("/users/{?*+}/info")]
    [InlineData("/users/{+}")]
    [InlineData("/users/{*}")]
    [InlineData("/users/{-}")]
    public void ParamNamesIsValid_WhenParamNamesIsInvalid_ShouldReturnFalse(string pattern)
    {
        // Act
        var result = _validator.ParamNamesIsValid(pattern);
        
        // Assert
        Assert.False(result);
    }
    
    [Theory]
    [MemberData(nameof(ValidPatterns))]
    public void ParamNamesIsValid_WhenParamNamesIsValid_ShouldReturnTrue(string pattern)
    {
        // Act
        var result = _validator.ParamNamesIsValid(pattern);
        
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ParamsCountIsValid_WhenParamsCountIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var pattern = "/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}/{p11}/{p12}/{p13}/{p14}/{p15}/{p16}/{p17}";
        
        // Act
        var result = _validator.ParamsCountIsValid(pattern);
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ParamsCountIsValid_WhenParamsCountIsValid_ShouldReturnTrue()
    {
        // Arrange
        var pattern = "/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}/{p11}/{p12}/{p13}/{p14}/{p15}/{p16}";
        
        // Act
        var result = _validator.ParamsCountIsValid(pattern);
        
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ParamsCountIsEqual_WhenParamsCountIsNotEqual_ShouldReturnFalse()
    {
        // Arrange
        var upstream = "/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}/{p11}/{p12}/{p13}/{p14}/{p15}/{p16}";
        var downstream = "/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}/{p11}/{p12}/{p13}/{p14}/{p15}";
        
        // Act
        var result = _validator.ParamsCountIsEqual(upstream, downstream);
        
        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void ParamsCountIsEqual_WhenParamsCountIsEqual_ShouldReturnTrue()
    {
        // Arrange
        var upstream = "/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}/{p11}/{p12}/{p13}/{p14}/{p15}/{p16}";
        var downstream = "/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}/{p11}/{p12}/{p13}/{p14}/{p15}/{p16}";
        
        // Act
        var result = _validator.ParamsCountIsEqual(upstream, downstream);
        
        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("/users/{id}/{id}")]
    [InlineData("/users/{id}/info/{id}")]
    [InlineData("/users/{id}/info/{name}/test/{id}")]
    public void HasNoDuplicateParameters_WhenHasDuplicateParameters_ShouldReturnFalse(string pattern)
    {
        // Act
        var result = _validator.HasNoDuplicateParameters(pattern);
        
        // Assert
        Assert.False(result);
    }
    
    [Theory]
    [MemberData(nameof(ValidPatterns))]
    public void HasNoDuplicateParameters_WhenHasNoDuplicateParameters_ShouldReturnTrue(string pattern)
    {
        // Act
        var result = _validator.HasNoDuplicateParameters(pattern);
        
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HttpMethodsIsValid_WhenHttpMethodIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var methods = new [] { "GET", "POST", "PUT", "TEST" };
        
        // Act
        var result = _validator.HttpMethodsIsValid(methods);
        
        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void HttpMethodsIsValid_WhenHttpMethodIsValid_ShouldReturnTrue()
    {
        // Arrange
        var methods = new [] { "GET", "POST", "PUT", "PATCH", "DELETE", "HEAD", "OPTIONS" };
        
        // Act
        var result = _validator.HttpMethodsIsValid(methods);
        
        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public void HasNoDuplicateMethods_WhenHasDuplicateMethods_ShouldReturnFalse()
    {
        // Arrange
        var methods = new [] { "GET", "POST", "PUT", "GET" };
        
        // Act
        var result = _validator.HasNoDuplicateMethods(methods);
        
        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void HasNoDuplicateMethods_WhenHasNoDuplicateMethods_ShouldReturnTrue()
    {
        // Arrange
        var methods = new [] { "GET", "POST", "PUT", "GET" };
        
        // Act
        var result = _validator.HasNoDuplicateMethods(methods);
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void MethodUpstreamCombinationIsUnique_WhenMethodUpstreamCombinationIsNotUnique_ShouldReturnFalse()
    {
        // Arrange
        var routes = new[]
        {
            new RouteCfg
            {
                Methods = [ "GET" ],
                UpstreamPattern = "/users",
            },
            new RouteCfg
            {
                Methods = [ "GET", "POST" ],
                UpstreamPattern = "/users/{id}",
            },
            new RouteCfg
            {
                Methods = [ "DELETE" ],
                UpstreamPattern = "/users/{id}",
            },
            new RouteCfg
            {
                Methods = [ "GET" ],
                UpstreamPattern = "/users/{id}",
            }
        };
        
        // Act
        var result = _validator.MethodUpstreamCombinationIsUnique(routes[1], routes);
        
        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void MethodUpstreamCombinationIsUnique_WhenMethodUpstreamCombinationIsUnique_ShouldReturnTrue()
    {
        // Arrange
        var routes = new[]
        {
            new RouteCfg
            {
                Methods = [ "GET" ],
                UpstreamPattern = "/users",
            },
            new RouteCfg
            {
                Methods = [ "GET", "POST" ],
                UpstreamPattern = "/users/{id}",
            },
            new RouteCfg
            {
                Methods = [ "DELETE" ],
                UpstreamPattern = "/users/{id}",
            },
            new RouteCfg
            {
                Methods = [ "PATCH" ],
                UpstreamPattern = "/users/{id}",
            }
        };
        
        // Act
        var result = _validator.MethodUpstreamCombinationIsUnique(routes[1], routes);
        
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void TotalDownstreamPathsLengthIsValid_WhenUpstreamPathsLengthIsInvalid_ShouldReturnFalse()
    {
        // Arrange
        var routes = new[]
        {
            new RouteCfg
            {
                DownstreamPattern = new string('x', 32768)
            },
            new RouteCfg
            {
                DownstreamPattern = new string('x', 32768)
            }
        };
        

        // Act
        var result = _validator.TotalDownstreamPathsLengthIsValid(routes); // суммарная длина около 66к
        
        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public void TotalDownstreamPathsLengthIsValid_WhenUpstreamPathsLengthIsValid_ShouldReturnTrue()
    {
        // Arrange
        var routes = new[]
        {
            new RouteCfg
            {
                DownstreamPattern = new string('x', 32767)
            },
            new RouteCfg
            {
                DownstreamPattern = new string('x', 32767)
            }
        };
        
        // Act
        var result = _validator.TotalDownstreamPathsLengthIsValid(routes);
        
        // Assert
        Assert.True(result);
    }
}