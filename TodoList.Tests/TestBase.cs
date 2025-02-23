namespace TodoList.Tests;


public abstract class TestBase
{
    protected static ClaimsPrincipal CreateTestUser(string userId = "test-user-id")
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        return new ClaimsPrincipal(identity);
    }

    protected static ControllerContext CreateControllerContext(ClaimsPrincipal? user = null)
    {
        var httpContext = new DefaultHttpContext();
        if (user != null)
        {
            httpContext.User = user;
        }
        return new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    protected static string CreateTestBearerToken()
    {
        return "test-bearer-token";
    }
}