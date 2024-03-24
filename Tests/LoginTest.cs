using Microsoft.EntityFrameworkCore;
using Server.App.Db.Contexts;
using Server;

namespace Tests;
// Hello f*ckig world
public class LoginTests : IClassFixture<DatabaseFixure>
{
    DatabaseFixure fixure;
    private LoginService _loginService;

    public LoginTests(DatabaseFixure fixure)
    {
        this.fixure = fixure;
        _loginService = new LoginService(fixure.Db, new HashService());
    }
    
    [Fact]
    public void Login()
    {
        var res = _loginService.Login("as", "123");
        Assert.True(fixure.Db.Users.Contains(res));
    }

    [Fact]
    public void NoLoginLogin_ThrowsArgumentException()
    {
        var act = () => _loginService.Login("as32", "123");
        var exception = Assert.Throws<ArgumentException>(act);
        Assert.Equal("There is no user with such password or login", exception.Message);
    }
    
    [Fact]
    public void WrongPasswordLogin_ThrowsArgumentException()
    {
        var act = () => _loginService.Login("as", "1134");
        var exception = Assert.Throws<ArgumentException>(act);
        Assert.Equal("There is no user with such password or login", exception.Message);
    }
}