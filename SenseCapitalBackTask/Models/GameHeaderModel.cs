using SenseCapitalBackTask.Data;

namespace SenseCapitalBackTask.Models
{
    public class GameHeaderModel
    {
        public int Id { get; set; } // Идентификатор игры
        
        public string EnemyName { get; set; } // Имя соперника

        public bool IsNew { get; set; } // Новая ли игра
    }
}
