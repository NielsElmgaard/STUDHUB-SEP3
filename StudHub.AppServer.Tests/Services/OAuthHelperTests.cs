using System.Text;
using Xunit;
using Studhub.AppServer.Services; // Assuming OAuthHelper is in this namespace

namespace Studhub.AppServer.Tests.Services;

/// <summary>
/// Unit tests for the pure logic inside the OAuthHelper.UrlEncode method.
/// </summary>
public class OAuthHelperTests
{
    // ZOMBIE: Many / Interface
    [Fact]
    public void UrlEncode_UnreservedChars_ReturnsUnchanged()
    {
        // RFC 3986 Unreserved Characters: ALPHA / DIGIT / "-" / "." / "_" / "~"
        string unreserved = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._~";
        
        string encoded = OAuthHelper.UrlEncode(unreserved);

        // Verify that the characters defined as 'unreserved' in the code are not altered.
        Assert.Equal(unreserved, encoded);
    }

    // ZOMBIE: Zero
    [Fact]
    public void UrlEncode_EmptyString_ReturnsEmpty()
    {
        string encoded = OAuthHelper.UrlEncode(string.Empty);

        // Verify the 'Zero' case: an empty string input yields an empty string output.
        Assert.Equal(string.Empty, encoded);
    }

    // ZOMBIE: One / Boundary
    [Fact]
    public void UrlEncode_SpaceChar_ReturnsPercent20()
    {
        string input = " "; 
        string expected = "%20";
        
        string encoded = OAuthHelper.UrlEncode(input);

        // Verify the 'One' case and that a space is correctly encoded as %20 (required by OAuth 1.0a).
        Assert.Equal(expected, encoded);
    }

    // ZOMBIE: Many / Boundary (Reserved Chars)
    [Fact]
    public void UrlEncode_SpecialReservedCharacters_AreEncoded()
    {
        // Characters critical for query strings and OAuth parameters that must be encoded.
        string input = "&=*#@+$/:,;?";
        string expected = "%26%3D%2A%23%40%2B%24%2F%3A%2C%3B%3F";
        
        string encoded = OAuthHelper.UrlEncode(input);

        // Verify that all reserved characters in the test string are correctly converted to their hexadecimal representation.
        Assert.Equal(expected, encoded);
    }

    // ZOMBIE: Many / Boundary (Mixed Chars)
    [Fact]
    public void UrlEncode_MixedCase_EncodesReservedButKeepsUnreserved()
    {
        string input = "v1/catalog/lookup?key=123&boid=3623";
        string expected = "v1%2Fcatalog%2Flookup%3Fkey%3D123%26boid%3D3623";
        
        string encoded = OAuthHelper.UrlEncode(input);

        // Verify that the logic correctly applies encoding only to reserved characters within a complex string.
        Assert.Equal(expected, encoded);
    }

    // ZOMBIE: Boundary (Non-ASCII Char Set)
    [Fact]
    public void UrlEncode_NonAsciiCharacters_AreEncoded()
    {
        // Testing characters outside the standard ASCII range.
        string inputSimple = "é";
        // Expected based on the provided implementation's logic (single-byte character code 0xE9):
        string expectedSimple = "%E9"; 

        string encodedSimple = OAuthHelper.UrlEncode(inputSimple);
        
        // Verify that the high ASCII character is encoded based on its single-byte character code.
        Assert.Equal(expectedSimple, encodedSimple); 
    }
    
    // ZOMBIE: Errors (Null Input)
    [Fact]
    public void UrlEncode_NullString_ThrowsArgumentNullException()
    {
        // Verify the 'Errors' case: passing a null value results in an exception.
        Assert.Throws<ArgumentNullException>(() => OAuthHelper.UrlEncode(null));
    }
}