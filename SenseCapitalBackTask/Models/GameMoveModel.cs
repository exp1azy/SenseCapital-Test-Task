namespace SenseCapitalBackTask.Models
{
    public class GameMoveModel
    {
        public int GameId { get; set; } // Идентификатор игры

        public int PlayerId { get; set; } // Идентификатор игрока

        public int Row { get; set; } // Идентификатор строки

        public int Col { get; set; } // Идентификатор колонки
    }
}
