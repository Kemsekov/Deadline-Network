using Microsoft.EntityFrameworkCore;
using Server;
using Server.App.Db.Contexts;

public interface IUserGroup{
    /// <summary>
    /// Делаем join в базу данных по userId в таблицу user_group,
    /// делаем ещё один join в таблицу groups, и возвращаем список групп в
    /// которых состоит юзер.
    /// 
    /// Если юзера с таким id нет в бд, или у него нет групп, возвращаем 
    /// Enumerable.Empty<Group>();
    /// </summary>
    Task<IEnumerable<Group>> GetUserGroups(int userId);
    /// <summary>
    /// Делаем join в бд из таблицы group по groupId в user_group,
    /// Делаем join в User - получаем список юзеров в группе.
    /// 
    /// Если группы с таким id нет в бд то кинуть исключение, 
    /// если у группы нет юзеров, возвращаем 
    /// Enumerable.Empty<User>();
    /// </summary>
    Task<IEnumerable<User>> GetGroupUsers(int groupId);
}

public class UserGroupManager : IUserGroup
{
    public ApplicationDbContext Db { get; }
    public UserGroupManager(ApplicationDbContext db)
    {
        Db = db;
    }

    public async Task<IEnumerable<Group>> GetUserGroups(int userId)
    {
        var groups = await Db.UserGroups.Join(Db.Groups,
            ug => ug.GroupId,
            g => g.Id,
            (ug, g) => new
            {
                userId = ug.UserId,
                group = ug.Group
            })
            .Where(u => u.userId == userId)
            .Select(p => new Group
            {
                Id = p.group.Id,
                Name = p.group.Name,
                PasswordHash = p.group.PasswordHash
            })
            .ToListAsync();
        if (groups is null)
        {
            return Enumerable.Empty<Group>();
        }
        else
        {
            return groups;
        }
    }

    public async Task<IEnumerable<User>> GetGroupUsers(int groupId)
    {
        var users = await Db.UserGroups.Include(u => u.User)
            .Where(u => u.GroupId == groupId)
            .Select(u => new User
            {
                Id = u.User.Id,
                Name = u.User.Name,
                LoginHash = u.User.LoginHash,
                PasswordHash = u.User.PasswordHash
            })
            .ToListAsync();
        if (users is null)
        {
            return Enumerable.Empty<User>();
        }
        else
        {
            return users;
        }
    }


}