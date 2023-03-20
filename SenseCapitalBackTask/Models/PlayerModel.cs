using SenseCapitalBackTask.Data;

namespace SenseCapitalBackTask.Models
{
    public class PlayerModel
    {
        public int Id { get; set; } // Идентификатор игрока

        public string Name { get; set; } // Имя игрока

        public string Password { get; set; } // Пароль игрока

        public static PlayerModel? Map(Player player) => player == null ? null : new PlayerModel // Возвращает модель таблицы
        {
            Id = player.Id,
            Name = player.Name,
            Password = player.Pass
        };
    }
}
