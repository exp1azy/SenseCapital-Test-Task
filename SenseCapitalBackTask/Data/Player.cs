using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SenseCapitalBackTask.Data
{
    [Table("players")]
    public class Player
    {
        [Key]
        [Column("player_id")]
        public int Id { get; set; }

        [Column("player_name")]
        public string Name { get; set; }

        [Column("player_password")]
        public string Pass { get; set; }
    }
}