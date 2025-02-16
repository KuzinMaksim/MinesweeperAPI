using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MinesweeperAPI.Models
{
    public class GameTurnRequest
    {
        [JsonPropertyName("game_id")]
        public Guid GameID { get; set; }
        [JsonPropertyName("col")]
        public int Column { get; set; }

        [JsonPropertyName("row")]
        public int Row { get; set; }

    }
}
