using Microsoft.EntityFrameworkCore;
using Server.App.Db.Contexts;
using Server;
using Microsoft.EntityFrameworkCore.Storage;

namespace Tests;

public class DatabaseFixure: IDisposable
{
    public ApplicationDbContext Db { get; private set; }
    public DatabaseFixure()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase", new InMemoryDatabaseRoot())
            .Options;

        Db = new ApplicationDbContext(options);
        Db.Users.Add(new User
        {
            Name = "vlad",
            Id = 1,
            LoginHash="as",
            PasswordHash="123"
        });
        Db.Users.Add(new User
        {
            Name = "dima",
            Id = 2,
            LoginHash="sd",
            PasswordHash="123"
        });
        Db.Users.Add(new User
        {
            Name = "sasha",
            Id = 3,
            LoginHash="asd",
            PasswordHash="123"
        });
        Db.Groups.Add(new Group{
            Id=1,
            Name="group 1",
            PasswordHash="123"
        });
        Db.Groups.Add(new Group{
            Id=2,
            Name="group 2",
            PasswordHash="123"
        });
        Db.UserGroups.Add(new UserGroup{
            GroupId=1,
            UserId=1,
            IsOwner=true
        });
        Db.UserGroups.Add(new UserGroup{
            GroupId=1,
            UserId=2,
            IsOwner=false
        });
        Db.UserGroups.Add(new UserGroup{
            GroupId=2,
            UserId=1,
            IsOwner=false
        });
        Db.SaveChanges();
    }
    public void Dispose()
    {
        Db.Database.EnsureDeleted();
    }
}