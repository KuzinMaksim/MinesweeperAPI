using System.Text.Json.Serialization;

namespace MinesweeperAPI.Models
{
    public class NewGameRequest
    {
        [JsonPropertyName("width")]
        public int Width { get; set; }
        [JsonPropertyName("height")]
        public int Height { get; set; }
        [JsonPropertyName("mines_count")]
        public int MinesCount { get; set; }
    }
}
