using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SenseCapitalBackTask.Data
{
    [Table("game")]
    public class Game
    {
        [Key]
        [Column("game_id")]
        public int Id { get; set; }

        [Column("zero_id")]
        public int ZeroPlayerId { get; set; }

        [Column("cross_id")]
        public int CrossPlayerId { get; set; }

        [Column("whose")]
        public int Whose { get; set; }

        [ForeignKey("ZeroPlayerId")]
        public Player ZeroPlayer { get; set; }

        [ForeignKey("CrossPlayerId")]
        public virtual Player CrossPlayer { get; set; }

        public virtual ICollection<Cell> Cells { get; set; }
    }
}