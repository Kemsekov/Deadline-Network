using Microsoft.EntityFrameworkCore;

namespace Tests;

public class JoinGroupTest : IClassFixture<DatabaseFixure>
{
    DatabaseFixure fixure;
    private JoinGroup _joinGroup;

    public JoinGroupTest(DatabaseFixure fixure)
    {
        this.fixure = fixure;
        _joinGroup = new JoinGroup(this.fixure.Db, new HashService());
    }

    [Fact]
    public void Join()
    {
        var res = _joinGroup.Join(3, 2, "123").Result;
        Assert.True(fixure.Db.UserGroups.Contains(res));
    }
    [Fact]
    public async void NoUserJoin_ThrowsArgumentException()
    {
        var act = () => _joinGroup.Join(5, 2, "123");
        var exception = await Assert.ThrowsAsync<ArgumentException>(act);
        Assert.Equal("No such user", exception.Message);
    }
    [Fact]
    public async void NoGroupJoin_ThrowsArgumentException()
    {
        var act = () => _joinGroup.Join(3, 5, "123");
        var exception = await Assert.ThrowsAsync<ArgumentException>(act);
        Assert.Equal("No such group", exception.Message);
    }
    [Fact]
    public async void UserAlreadyInGroupJoin_ThrowsArgumentException()
    {
        var act = () => _joinGroup.Join(1, 2, "123");
        var exception = await Assert.ThrowsAsync<ArgumentException>(act);
        Assert.Equal("User already in group", exception.Message);
    }
    [Fact]
    public async void WrongPasswordJoin_ThrowsArgumentException()
    {
        var act = () => _joinGroup.Join(3, 2, "1234");
        var exception = await Assert.ThrowsAsync<ArgumentException>(act);
        Assert.Equal("Wrong password", exception.Message);
    }
    
}