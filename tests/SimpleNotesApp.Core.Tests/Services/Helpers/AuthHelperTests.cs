using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using SimpleNotesApp.Core.Services.Helpers;

namespace SimpleNotesApp.Core.Tests.Services.Helpers;

public class AuthHelperTests
{
  private readonly AuthHelper _authHelper;
  private readonly Mock<IConfiguration> _configuration;

  public AuthHelperTests()
  {
    _configuration = new Mock<IConfiguration>();

    var passwordKeySection = new Mock<IConfigurationSection>();
    passwordKeySection.Setup(x => x.Value).Returns("test-password-key-that-is-long-enough-for-hmac");
    _configuration.Setup(x => x.GetSection("AppSettings:PasswordKey")).Returns(passwordKeySection.Object);

    var tokenKeySection = new Mock<IConfigurationSection>();
    tokenKeySection.Setup(x => x.Value).Returns("test-token-key-that-must-be-at-least-64-characters-long-for-hmacsha512");
    _configuration.Setup(x => x.GetSection("AppSettings:TokenKey")).Returns(tokenKeySection.Object);

    _authHelper = new AuthHelper(_configuration.Object);
  }

  #region IsValidEmail Tests

  [Theory]
  [InlineData("test@example.com", true)]
  [InlineData("user.name@example.com", true)]
  [InlineData("user+tag@example.co.uk", true)]
  [InlineData("user_name@example.com", true)]
  [InlineData("123@example.com", true)]
  [InlineData("user@subdomain.example.com", true)]
  [InlineData("", false)]
  [InlineData(" ", false)]
  [InlineData("not-an-email", false)]
  [InlineData("@example.com", false)]
  [InlineData("user@", false)]
  [InlineData("user @example.com", false)]
  [InlineData("user@.com", false)]
  [InlineData("user..name@example.com", false)]
  public void IsValidEmail_VariousInputs_ReturnsExpectedResult(string email, bool expected)
  {
    var result = _authHelper.IsValidEmail(email);

    result.Should().Be(expected);
  }

  [Fact]
  public void IsValidEmail_NullEmail_ReturnsFalse()
  {
    var result = _authHelper.IsValidEmail(null!);

    result.Should().BeFalse();
  }

  #endregion

  #region GenerateOtp Tests

  [Fact]
  public void GenerateOtp_ReturnsValidSixDigitString()
  {
    var otp = _authHelper.GenerateOtp();

    otp.Should().NotBeNullOrEmpty();
    otp.Length.Should().Be(6);
    otp.Should().MatchRegex(@"^\d{6}$");
  }

  [Fact]
  public void GenerateOtp_CalledMultipleTimes_ReturnsDifferentValues()
  {
    var otp1 = _authHelper.GenerateOtp();
    var otp2 = _authHelper.GenerateOtp();
    var otp3 = _authHelper.GenerateOtp();

    var otps = new[] { otp1, otp2, otp3 };
    otps.Should().OnlyHaveUniqueItems();
  }

  [Fact]
  public void GenerateOtp_ReturnsNumericStringWithLeadingZeros()
  {
    var otps = new List<string>();
    for (int i = 0; i < 100; i++)
    {
      otps.Add(_authHelper.GenerateOtp());
    }

    foreach (var otp in otps)
    {
      otp.Length.Should().Be(6);
    }
    otps.Should().Contain(otp => otp.StartsWith('0'));
  }

  #endregion

  #region GetPasswordHash Tests

  [Fact]
  public void GetPasswordHash_SamePasswordAndSalt_ReturnsSameHash()
  {
    var password = "MySecurePassword123!";
    var salt = new byte[16];
    Array.Fill<byte>(salt, 1);

    var hash1 = _authHelper.GetPasswordHash(password, salt);
    var hash2 = _authHelper.GetPasswordHash(password, salt);

    hash1.Should().BeEquivalentTo(hash2);
  }

  [Fact]
  public void GetPasswordHash_DifferentPasswords_ReturnsDifferentHashes()
  {
    var password1 = "Password1";
    var password2 = "Password2";
    var salt = new byte[16];
    Array.Fill<byte>(salt, 1);

    var hash1 = _authHelper.GetPasswordHash(password1, salt);
    var hash2 = _authHelper.GetPasswordHash(password2, salt);

    hash1.Should().NotBeEquivalentTo(hash2);
  }

  [Fact]
  public void GetPasswordHash_DifferentSalts_ReturnsDifferentHashes()
  {
    var password = "MyPassword";
    var salt1 = new byte[16];
    var salt2 = new byte[16];
    Array.Fill<byte>(salt1, 1);
    Array.Fill<byte>(salt2, 2);

    var hash1 = _authHelper.GetPasswordHash(password, salt1);
    var hash2 = _authHelper.GetPasswordHash(password, salt2);

    hash1.Should().NotBeEquivalentTo(hash2);
  }

  [Fact]
  public void GetPasswordHash_ReturnsCorrectLength()
  {
    var password = "TestPassword";
    var salt = new byte[16];

    var hash = _authHelper.GetPasswordHash(password, salt);

    hash.Length.Should().Be(32);
  }

  #endregion

  #region CreateToken Tests

  [Fact]
  public void CreateToken_ValidUserId_ReturnsValidJwtToken()
  {
    var userId = 123;

    var token = _authHelper.CreateToken(userId);

    token.Should().NotBeNullOrEmpty();
    var handler = new JwtSecurityTokenHandler();
    handler.CanReadToken(token).Should().BeTrue();
  }

  [Fact]
  public void CreateToken_ValidUserId_TokenContainsUserIdClaim()
  {
    var userId = 456;

    var token = _authHelper.CreateToken(userId);

    var handler = new JwtSecurityTokenHandler();
    var jwtToken = handler.ReadJwtToken(token);
    var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "userId");

    userIdClaim.Should().NotBeNull();
    userIdClaim!.Value.Should().Be(userId.ToString());
  }

  [Fact]
  public void CreateToken_ValidUserId_TokenExpiresInOneHour()
  {
    var userId = 789;
    var beforeCreation = DateTime.UtcNow;

    var token = _authHelper.CreateToken(userId);

    var handler = new JwtSecurityTokenHandler();
    var jwtToken = handler.ReadJwtToken(token);
    var expectedExpiry = beforeCreation.AddHours(1);

    var timeDifference = Math.Abs((jwtToken.ValidTo - expectedExpiry).TotalSeconds);
    timeDifference.Should().BeLessThan(5);
  }

  [Fact]
  public void CreateToken_DifferentUserIds_ReturnsDifferentTokens()
  {
    var token1 = _authHelper.CreateToken(1);
    var token2 = _authHelper.CreateToken(2);

    token1.Should().NotBe(token2);
  }

  #endregion

  #region CreateRefreshToken Tests

  [Fact]
  public void CreateRefreshToken_ReturnsNonEmptyString()
  {
    var refreshToken = _authHelper.CreateRefreshToken();

    refreshToken.Should().NotBeNullOrEmpty();
  }

  [Fact]
  public void CreateRefreshToken_ReturnsUrlSafeBase64String()
  {
    var refreshToken = _authHelper.CreateRefreshToken();

    refreshToken.Should().NotContain("+");
    refreshToken.Should().NotContain("/");
    refreshToken.Should().NotContain("=");
    refreshToken.Should().MatchRegex(@"^[A-Za-z0-9_-]+$");
  }

  [Fact]
  public void CreateRefreshToken_CalledMultipleTimes_ReturnsDifferentTokens()
  {
    var token1 = _authHelper.CreateRefreshToken();
    var token2 = _authHelper.CreateRefreshToken();
    var token3 = _authHelper.CreateRefreshToken();

    var tokens = new[] { token1, token2, token3 };
    tokens.Should().OnlyHaveUniqueItems();
  }

  [Fact]
  public void CreateRefreshToken_ReturnsTokenOfExpectedLength()
  {
    var refreshToken = _authHelper.CreateRefreshToken();

    refreshToken.Length.Should().BeGreaterThan(80);
  }

  #endregion
}
