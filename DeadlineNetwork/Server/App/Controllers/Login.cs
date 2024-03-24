using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Server;
using Server.App.Db.Contexts;

public interface ILogin
{
    /// <summary>
    /// Принимая на вход  логин и пароль, 
    /// хеширует их, 
    /// ищет в бд есть ли такой юзер
    /// Если есть возвращает найденный объект
    /// Иначе кидает exception с ошибкой не правильный логин или пароль
    /// </summary>
    User Login(string login, string password);
}

public interface IHash
{
    string Hash(string data);
}

public class LoginService : ILogin
{
    public ApplicationDbContext db { get; }
    public IHash hashService;
    public LoginService(ApplicationDbContext db, IHash hashService)
    {
        this.db = db;
        this.hashService = hashService;
    }

    public User Login(string login, string password)
    {
        string loginHash = hashService.Hash(login);
        string passwordHash = hashService.Hash(password);
        var user = db.Users.Where(p => p.PasswordHash == passwordHash && p.LoginHash == loginHash);
        if (user.Count() == 0)
            throw new ArgumentException("There is no user with such password or login");
        else
        {
            return user.First();
        }
    }
}