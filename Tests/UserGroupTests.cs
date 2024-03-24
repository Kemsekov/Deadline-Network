using Microsoft.EntityFrameworkCore;
using Server.App.Db.Contexts;
using Server;

namespace Tests;
// Hello f*ckig world
public class UserGroupManagerTests : IClassFixture<DatabaseFixure>
{
    DatabaseFixure fixure;
    private UserGroupManager _userGroupManager;

    public UserGroupManagerTests(DatabaseFixure fixure)
    {
        this.fixure = fixure;
        _userGroupManager = new UserGroupManager(this.fixure.Db);
    }
    [Fact]
    public void GetUserGroups()
    {
        var res = _userGroupManager.GetUserGroups(1).Result;
        Assert.True(res.Count() == 2);
    }
    [Fact]
    public void GetGroupUsers()
    {
        var res = _userGroupManager.GetGroupUsers(1).Result;
        Assert.True(res.Count() == 2);
    }

    [Fact]
    public void NoUserGetUserGroups_Fails()
    {
        var res = _userGroupManager.GetUserGroups(34).Result;
        Assert.Empty(res);
    }

    [Fact]
    public void NoGroupsGetUserGroups_Fails()
    {
        var res = _userGroupManager.GetUserGroups(3).Result;
        Assert.Empty(res);
    }
}