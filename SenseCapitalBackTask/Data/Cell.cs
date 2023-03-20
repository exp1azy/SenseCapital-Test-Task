using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SenseCapitalBackTask.Data
{
    [Table("cell")]
    [PrimaryKey(nameof(GameId), nameof(Row), nameof(Col))]
    public class Cell
    {
        [Column("game_id")]
        public int GameId { get; set; }

        [Column("cell_row")]
        public int Row { get; set; }

        [Column("cell_col")]
        public int Col { get; set; }

        [Column("cell_state")]
        public bool? State { get; set; }

        [Column("mark")]
        public bool Mark { get; set; }
    }
}