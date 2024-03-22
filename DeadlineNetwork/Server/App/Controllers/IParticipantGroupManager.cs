using Server.App.Db.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.App.Controllers
{
    /// <summary>
    /// Менеджер группы для участников группы.
    /// </summary>
    public class GroupMemberManager : IParticipantGroupManager
    {
        private readonly ApplicationDbContext _db; // Контекст базы данных
        public User Participant { get; } // Участник группы
        public Group Group { get; } // Группа

        // Конструктор класса
        public GroupMemberManager(ApplicationDbContext db, User participant, Group group)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db)); // Инициализация контекста базы данных
            Participant = participant ?? throw new ArgumentNullException(nameof(participant)); // Инициализация участника группы
            Group = group ?? throw new ArgumentNullException(nameof(group)); // Инициализация группы

            // Проверка, что пользователь является участником группы
            if (!_db.UserGroups.Any(ug => ug.UserId == participant.Id && ug.GroupId == group.Id))
            {
                throw new Exception("Пользователь не является участником группы.");
            }
        }

        // Получение списка дисциплин группы
        public IEnumerable<Discipline> GetDisciplines(int groupId)
        {
            return _db.Disciplines.Where(d => d.GroupId == groupId).AsEnumerable();
        }

        // Добавление задачи в дисциплину
        public Server.Task AddTask(int disciplineId, string description, string comment, DateTime deadline)
        {
            // Проверка на пустое описание
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Описание задачи не может быть пустым.", nameof(description));

            // Проверка, что срок выполнения не в прошлом
            if (deadline < DateTime.UtcNow)
                throw new ArgumentException("Срок выполнения задачи не может быть в прошлом.", nameof(deadline));

            // Поиск дисциплины по ID
            var discipline = _db.Disciplines.Find(disciplineId) ?? throw new Exception("Дисциплина не найдена.");

            // Создание новой задачи
            var task = new Server.Task
            {
                DisciplineId = disciplineId,
                Description = description,
                Comment = comment,
                Deadline = deadline.ToUniversalTime(), // Преобразование даты в UTC
                Created = DateTime.UtcNow, // Установка текущей даты создания
                WhoAdded = Participant.Id // ID участника, добавившего задачу
            };

            // Добавление задачи в базу данных
            _db.Tasks.Add(task);
            _db.SaveChanges(); // Сохранение изменений в базе данных

            return task; // Возврат созданной задачи
        }

        // Обновление задачи
        public Server.Task UpdateTask(int taskId, string description, string comment, DateTime deadline)
        {
            // Проверка на пустое описание
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Описание задачи не может быть пустым.", nameof(description));

            // Проверка, что срок выполнения не в прошлом
            if (deadline < DateTime.UtcNow)
                throw new ArgumentException("Срок выполнения задачи не может быть в прошлом.", nameof(deadline));

            // Поиск задачи по ID
            var task = _db.Tasks.Find(taskId) ?? throw new Exception("Задача не найдена.");

            // Проверка прав на обновление задачи
            if (Group.OwnerId == Participant.Id || task.WhoAdded == Participant.Id)
            {
                task.Description = description; // Обновление описания
                task.Comment = comment; // Обновление комментария
                task.Deadline = deadline.ToUniversalTime(); // Обновление срока выполнения

                _db.Tasks.Update(task); // Обновление задачи в базе данных
                _db.SaveChanges(); // Сохранение изменений в базе данных

                return task; // Возврат обновленной задачи
            }
            else
            {
                throw new Exception("Только создатель задачи может обновить её.");
            }
        }
    }
}
