using Server;
using Server.App.Db.Contexts;

public interface IRegister{
    /// <summary>
    /// Принимая на вход логин, пароль и имя
    /// хеширует логин и пароль
    /// Проверяет существование логина в базе данных
    /// Если логин есть, кидает Exception
    /// Создает новый объект User с полученными данными
    /// Добавляет созданный юзер в бд
    /// Возврашает созданный объект
    /// </summary>
    Task<User> Register(string login, string password, string name);
}


public class RegistrationService : IRegister
{
    public ApplicationDbContext Db { get; }
    public IHash hashService;
    public RegistrationService(ApplicationDbContext db, IHash hashService)
    {
        Db = db;
        this.hashService = hashService;
    }
    
    public async Task<User> Register(string login, string password, string userName)
    {
        string loginHash = hashService.Hash(login);
        string passwordHash = hashService.Hash(password);
        var userExist = Db.Users.Where(p => p.LoginHash == loginHash);
        if (userExist.Count() > 0)
            throw new ArgumentException("User with this login is already exists");
        var user = new User
        {
            Name = userName,
            PasswordHash = passwordHash,
            LoginHash = loginHash
        };

        await Db.Users.AddAsync(user);
        await Db.SaveChangesAsync();
        return user;
    } 
}