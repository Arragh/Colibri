using Colibri.Services.ConfigValidator;

namespace Unit.Validator;

public class JwtSchemeValidatorTests
{
    private readonly JwtSchemeValidator _validator = new();
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void AuthAlgorithmIsNotEmpty_WhenAuthAlgorithmIsEmpty_ShouldReturnFalse(string algorithm)
    {
        // Act
        var result = _validator.AuthAlgorithmIsNotEmpty(algorithm);
        
        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("rs256")]
    [InlineData("hs256")]
    [InlineData("es256")]
    [InlineData("BELIBERDA")]
    public void AuthAlgorithmIsNotEmpty_WhenAuthAlgorithmIsNotEmpty_ShouldReturnTrue(string algorithm)
    {
        // Act
        var result = _validator.AuthAlgorithmIsNotEmpty(algorithm);
        
        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("rs255")]
    [InlineData("ES221")]
    [InlineData("hs512")]
    [InlineData("BELIBERDA")]
    public void AuthAlgorithmIsValid_WhenAuthAlgorithmIsInvalid_ShouldReturnFalse(string algorithm)
    {
        // Act
        var result = _validator.AuthAlgorithmIsValid(algorithm);
        
        // Assert
        Assert.False(result);
    }
    
    [Theory]
    [InlineData("rs256")]
    [InlineData("hs256")]
    [InlineData("es256")]
    public void AuthAlgorithmIsValid_WhenAuthAlgorithmIsValid_ShouldReturnTrue(string algorithm)
    {
        // Act
        var result = _validator.AuthAlgorithmIsValid(algorithm);
        
        // Assert
        Assert.True(result);
    }

}