using Colibri.Configuration.Models;
using Colibri.Runtime.Pipeline.Cluster.Authorization;

namespace Unit.Authorization;

public class AuthorizerTests
{
    private const string algorithm = "rs256";
    private const string publicKey = "MIIBCgKCAQEAqeY7iu4DUqc+KIbJgXUVqu1mGT4rQY8A5f82C2f4+Hvd1c7JMp6NU+9v+ZxHAgakNIBSS8KL11X03a15NYN+W4lf4ujlLf5bKEGn5wOfOyIIUturpQkMg5dXW0ZzwO2NzqYwjGJHfONAwUWYbk/XsqQBXnUCFqU84atciyJwqJbVsQHOONaI+6KR/11K0qzHBxZUJSgxyeQEFFlOP51UYZJOWjVMjOIuNGNe4oZU80lzLQWezRtCenMPs+aDarXvsPUnsBLQv7piD+eu5uuaUF1p/iST+FXCIQYtGMWHRywiI1xHKiTBiUq15WNNl2bCaZWYktsk3vqr/qK5N151QQIDAQAB";
    private const string privateKey = "MIIEowIBAAKCAQEAqeY7iu4DUqc+KIbJgXUVqu1mGT4rQY8A5f82C2f4+Hvd1c7JMp6NU+9v+ZxHAgakNIBSS8KL11X03a15NYN+W4lf4ujlLf5bKEGn5wOfOyIIUturpQkMg5dXW0ZzwO2NzqYwjGJHfONAwUWYbk/XsqQBXnUCFqU84atciyJwqJbVsQHOONaI+6KR/11K0qzHBxZUJSgxyeQEFFlOP51UYZJOWjVMjOIuNGNe4oZU80lzLQWezRtCenMPs+aDarXvsPUnsBLQv7piD+eu5uuaUF1p/iST+FXCIQYtGMWHRywiI1xHKiTBiUq15WNNl2bCaZWYktsk3vqr/qK5N151QQIDAQABAoIBAES8QoaYeHut8bXPoiJfzh5S4SWBQ1rIkiJ6t9CxhcZxxnPDrx5titvUWMbxdMDbEv+ykpjX4l+CBQjSL+F4i1xZHZPdiSqsZxDITzdk/bycriVnfe/M02VBizQAMsBw2xgpoCaLdESQZBhNIbgvUSKSu4MNb4Td4N2jyFL54f4yLkN3W+7yvdpHpbuNEWmgcSlgnGeKy+0nNlbWi6/aQraMssoTNxWzRyhShyf/Mz3+Sm3izFpj7Vu9yCG33OjwVHZmmfvGeW2oAHerVO3jy0wVmcS7QqpDHqtsYH8j7oeQFrsR22K+Q6CqV6mv6Bh2bNXrsCJQafn37UHjaffyi8kCgYEA5cIYU20FcnsvZfJLcN01mKAq+pmGI2qJKBNECt25ivlcljTF8TykwriGs3ySpPtij4AH/AYS2Eby2X/dvoPELve5RM9I1XypQnH5Y7bXSiB0IFDGSh8b4pAEu8AbIsUQKoLVa2SPtYtwLfTvPdZd3fv1+u8MaSNuWSwORR6YrmcCgYEAvU3scKLGeoo04EjAPFbjgX9AuU4UJeSp+sjmbhfuTjecYW8zhXqVn0dSLeTWjhr+Yt9mFZYnuD0CijWAfQ8jDvZ+/JiaCoQcPsScn6PviNOAaeJO05hnEgAW0gAVffvWRc3momnmKrmcRUdptpMIL0DWV2NX+Hrb1SHJCZrwphcCgYEA32xZn3ba3zKvKuHtPrm+TsciRENaz8Hf0pq5hh+LOHu471AuVKJ2TeyHAK9ZIxpaA9Wq7gcx+CM2MQ9IoOUdt9ap0SIJX6E0LysKK084GBtrEam6yYVq5mzKTZFIUg1z5QMJar+FiDMqSWZv56A384/66aPgW7Var1hKICjarG0CgYBoYpGgQoHDjHY/vp4SJ69n9u2PwNgnVQHOAf3ec1+6zbtzlsgIMGJU5BUSrX8J+SSRDLLT8GqSk0HVSE9DppckZxP64YL/jX2ttujOtq0c+9Nxj5L75qvfJyFuRxGS2M18zF8C6/Y5VQQwx4IpZMY8mDoZEfSuVcDms6yRmVhdnQKBgASkp9A1bF/VSQ3hcw0ekFiTGD3Cg7dVI0FSKCgmROGpyerEHcbVCQ2/+8DVUt26ECiBWWSUvU9M5GcIsWRpFTa8dzrN7Kx6ERfrLIfiarJutCnxCEKbE30k0wJxeAjRQ85qIadRW4/l7nCACRjDn9/ZjJw2/kl0RhOAXswQv0UR";
    
    [Fact]
    public async Task ValidateToken_WhenTokenSigningKeyIsInvalid_ReturnsFalse()
    {
        // Arrange
        var claims = Array.Empty<ClaimCfg>();
        var authorizer = new Authorizer(claims, algorithm, publicKey);
        var token = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjoibW9kZXJhdG9yIiwiZXhwIjoyMDg5MjE2NTA2LCJpc3MiOiJvbG9sbyIsImF1ZCI6InRyb2xvbG8ifQ.1p-u_Tv2601uB-oMCC-uBym7DHB-IMGQimws8ZVWbwzIe11TxZK7nq3zLYKBZmGvlKWKduk68dtB6Emn-7aQ8VCbvL02Y2tH-FS6MwQyg9J8VJN2Zek1N_bOp6WhqfbPgbXSTR1BJAHBe8lM4imkBD3RSZ8hCnO1McZDycnRm7l7JBclORl5_l9-EbhdO8fHmKCKvgdEjAmux0Apoaq_tDfYCwWDGP1di7embeM_8F8kcr3Za-zbERhZ9TBGz39lddTSryzH9ecp0Nr8MMFHhjeBrOl1QsBu_2VvvTCfy6PC87QyxguSUWUYO0qVfLoEd_ifgjygkjywpSZyAWe1og";
        
        // Act
        var result = await authorizer.ValidateToken(token);
        
        // Assert
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public async Task ValidateToken_WhenTokenSigningKeyIsValid_ReturnsTrue()
    {
        // Arrange
        var claims = Array.Empty<ClaimCfg>();
        var authorizer = new Authorizer(claims, algorithm, publicKey);
        var token = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjoibW9kZXJhdG9yIiwiZXhwIjoyMDg5MjE3ODQ5LCJpc3MiOiJvbG9sbyIsImF1ZCI6InRyb2xvbG8ifQ.AmXZgwn686nC3h6p6BefENt9deYguyn3VNdVj5xoAcA6D8vKSkYAt1vvJU3VWwvEv7aAOYe2UuXpvTYUHsxhzhfoMODfllmK54ybcsJB4XxJfKRtjgV-DY9wVeMzfmIGAn8qxKkDoqaWlNUDIFXebya-PpMYPNmknlgJuIJbv026qJYuJsN03nqBapdZjdDLSXlAcGTO9-ojVoTXg0RaWC0m-ZcQwpaYIoN9sAadsRlJmmGTN3Jt3ofAFwmLOQOpGF-04O8FaajLCaWpkwi9NW6jgp6r2gf67YXXuVuEiYNADatnoxwcPB9cj212efB1uLvhBw4wBzxnHjS2W2BXnw";
        
        // Act
        var result = await authorizer.ValidateToken(token);
        
        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task TryAuthorize_WhenClaimIsSingleClaimIsInvalid_ReturnsFalse()
    {
        // Arrange
        ClaimCfg[] claims =
        [
            new()
            {
                Type = "role",
                Value = [ "admin" ]
            }
        ];
        var authorizer = new Authorizer(claims, algorithm, publicKey);
        // token has single claim role: moderator
        var token = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjoibW9kZXJhdG9yIiwiZXhwIjoyMDg5MjE3ODQ5LCJpc3MiOiJvbG9sbyIsImF1ZCI6InRyb2xvbG8ifQ.AmXZgwn686nC3h6p6BefENt9deYguyn3VNdVj5xoAcA6D8vKSkYAt1vvJU3VWwvEv7aAOYe2UuXpvTYUHsxhzhfoMODfllmK54ybcsJB4XxJfKRtjgV-DY9wVeMzfmIGAn8qxKkDoqaWlNUDIFXebya-PpMYPNmknlgJuIJbv026qJYuJsN03nqBapdZjdDLSXlAcGTO9-ojVoTXg0RaWC0m-ZcQwpaYIoN9sAadsRlJmmGTN3Jt3ofAFwmLOQOpGF-04O8FaajLCaWpkwi9NW6jgp6r2gf67YXXuVuEiYNADatnoxwcPB9cj212efB1uLvhBw4wBzxnHjS2W2BXnw";
        
        // Act
        var validateResult = await authorizer.ValidateToken(token);
        var authorizeResult = authorizer.TryAuthorize(validateResult.SecurityToken);
        
        // Assert
        Assert.False(authorizeResult);
    }
    
    [Fact]
    public async Task TryAuthorize_WhenClaimIsSingleClaimIsValid_ReturnsTrue()
    {
        // Arrange
        ClaimCfg[] claims =
        [
            new()
            {
                Type = "role",
                Value = [ "admin", "moderator" ]
            }
        ];
        var authorizer = new Authorizer(claims, algorithm, publicKey);
        // token has single claim role: moderator
        var token = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjoibW9kZXJhdG9yIiwiZXhwIjoyMDg5MjE3ODQ5LCJpc3MiOiJvbG9sbyIsImF1ZCI6InRyb2xvbG8ifQ.AmXZgwn686nC3h6p6BefENt9deYguyn3VNdVj5xoAcA6D8vKSkYAt1vvJU3VWwvEv7aAOYe2UuXpvTYUHsxhzhfoMODfllmK54ybcsJB4XxJfKRtjgV-DY9wVeMzfmIGAn8qxKkDoqaWlNUDIFXebya-PpMYPNmknlgJuIJbv026qJYuJsN03nqBapdZjdDLSXlAcGTO9-ojVoTXg0RaWC0m-ZcQwpaYIoN9sAadsRlJmmGTN3Jt3ofAFwmLOQOpGF-04O8FaajLCaWpkwi9NW6jgp6r2gf67YXXuVuEiYNADatnoxwcPB9cj212efB1uLvhBw4wBzxnHjS2W2BXnw";
        
        // Act
        var validateResult = await authorizer.ValidateToken(token);
        var authorizeResult = authorizer.TryAuthorize(validateResult.SecurityToken);
        
        // Assert
        Assert.True(authorizeResult);
    }
    
    [Fact]
    public async Task TryAuthorize_WhenClaimsAreArrayClaimIsInvalid_ReturnsFalse()
    {
        // Arrange
        ClaimCfg[] claims =
        [
            new()
            {
                Type = "role",
                Value = [ "moderator", "operator", "client" ]
            }
        ];
        var authorizer = new Authorizer(claims, algorithm, publicKey);
        // token has array of claims "role": [ "user", "admin" ]
        var token = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjpbInVzZXIiLCJhZG1pbiJdLCJleHAiOjIwODkyMTkwODQsImlzcyI6Im9sb2xvIiwiYXVkIjoidHJvbG9sbyJ9.SmW9yPuFOtbmlslk-0jY-kITyspr87MoR2XgsF65cGlxKAVV4MtmBU1o837ID9cEsodpVXUYbUGxqqJKZ-orFSty9_zBgOb_u2uJRpTVjSj9wCmWqXvb-i25aC3sy-Ss9il_ik55P-BsPxl9guME2LIO7V0lZ_tWBZvxTg7ZoVJnWl240dE0a3IxvbE3KdpaEstMrWr355d1y5MqEwnMSKmg0utasy8v6KYLwdiv5p7NrTFvayI1HROvYxWGhJuF4n6-ahbxx9ke39mqUVPCsrZZVlh7obY695fbina0HZCu_Qtz8Ls1NY5EpQjI4BLj4otkg4AeQ0q5_SUAJfV18g";
        
        // Act
        var validateResult = await authorizer.ValidateToken(token);
        var authorizeResult = authorizer.TryAuthorize(validateResult.SecurityToken);
        
        // Assert
        Assert.False(authorizeResult);
    }
    
    [Fact]
    public async Task TryAuthorize_WhenClaimsAreArrayClaimIsValid_ReturnsTrue()
    {
        // Arrange
        ClaimCfg[] claims =
        [
            new()
            {
                Type = "role",
                Value = [ "moderator", "admin", "donkey" ]
            }
        ];
        var authorizer = new Authorizer(claims, algorithm, publicKey);
        // token has array of claims "role": [ "user", "admin" ]
        var token = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjpbInVzZXIiLCJhZG1pbiJdLCJleHAiOjIwODkyMTkwODQsImlzcyI6Im9sb2xvIiwiYXVkIjoidHJvbG9sbyJ9.SmW9yPuFOtbmlslk-0jY-kITyspr87MoR2XgsF65cGlxKAVV4MtmBU1o837ID9cEsodpVXUYbUGxqqJKZ-orFSty9_zBgOb_u2uJRpTVjSj9wCmWqXvb-i25aC3sy-Ss9il_ik55P-BsPxl9guME2LIO7V0lZ_tWBZvxTg7ZoVJnWl240dE0a3IxvbE3KdpaEstMrWr355d1y5MqEwnMSKmg0utasy8v6KYLwdiv5p7NrTFvayI1HROvYxWGhJuF4n6-ahbxx9ke39mqUVPCsrZZVlh7obY695fbina0HZCu_Qtz8Ls1NY5EpQjI4BLj4otkg4AeQ0q5_SUAJfV18g";
        
        // Act
        var validateResult = await authorizer.ValidateToken(token);
        var authorizeResult = authorizer.TryAuthorize(validateResult.SecurityToken);
        
        // Assert
        Assert.True(authorizeResult);
    }
    
    [Fact]
    public async Task TryAuthorize_WhenClaimsAreEmpty_ReturnsFalse()
    {
        // Arrange
        ClaimCfg[] claims =
        [
            new()
            {
                Type = "role",
                Value = [ "moderator", "admin", "donkey" ]
            }
        ];
        var authorizer = new Authorizer(claims, algorithm, publicKey);
        // token has no any claims
        var token = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjIwODkyMTk3ODEsImlzcyI6Im9sb2xvIiwiYXVkIjoidHJvbG9sbyJ9.W5gEm6cT7KQ105d8B4NBoNOZA5urzyuiDW-GybUvkTl2wq8JZHukZx1ohYfEF63sf3W3WBfwQIyufVJnwkoUzSIVnXlm-a39WsK-umPgmRdv739m_c7Ft-D0Krvn7se2aBO_P3-XN4UVqQqpX6LhYVDLVIYekVb1HfE3KHb71QR3dVPRfJOkRt5JigRfz1NqV-hVFBV7nkud8DsjKmxD4TWJZe4lOKpNgMmjB17nnXaaU7BWaHOKyuD5PsufsGElg-_E7vBsZLK1SZ5b3XdGeZuaKkgKsnjKWTOSRq78XdTcN7qL7S6AmdU5BRMIrFOeFrj2FUk4oRUvqjHHAyRvvg";
        
        // Act
        var validateResult = await authorizer.ValidateToken(token);
        var authorizeResult = authorizer.TryAuthorize(validateResult.SecurityToken);
        
        // Assert
        Assert.False(authorizeResult);
    }
}