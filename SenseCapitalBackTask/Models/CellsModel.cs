using SenseCapitalBackTask.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace SenseCapitalBackTask.Models
{
    public class CellsModel
    {
        public int GameId { get; set; } // Идентификатор игры

        public int Row { get; set; } // Идентификатор строки

        public int Col { get; set; } // Идентификатор колонки

        public bool? State { get; set; } // Состояние клетки

        public bool Mark { get; set; } // Отмечена ли клетка (в случае выйгрыша)

        public static CellsModel? Map(Cell cells) => cells == null ? null : new CellsModel() // Возвращает модель таблицы
        {
            GameId = cells.GameId,
            Row = cells.Row,
            Col = cells.Col,
            State = cells.State,
            Mark = cells.Mark
        };
    }
}
