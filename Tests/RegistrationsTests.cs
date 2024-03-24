using Microsoft.EntityFrameworkCore;

namespace Tests;
// Hello f*ckig world
public class RegistrationServiceTest : IClassFixture<DatabaseFixure>
{
    DatabaseFixure fixure;
    private RegistrationService _registrationService;

    public RegistrationServiceTest(DatabaseFixure fixure)
    {
        this.fixure = fixure;
        _registrationService = new RegistrationService (this.fixure.Db, new HashService());
    }
    [Fact]
    public void Register()
    {
        var res = _registrationService.Register("vasya", "123", "vasya").Result;
        Assert.True(fixure.Db.Users.Contains(res));
    }
    [Fact]
    public async void LoginExistRegister_ThrowsArgumentException()
    {
        var act = () => _registrationService.Register("sd", "ds", "asd");
        var exception = await Assert.ThrowsAsync<ArgumentException>(act);
        Assert.Equal("User with this login is already exists", exception.Message);
    }
}
