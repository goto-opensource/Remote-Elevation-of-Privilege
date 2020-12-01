using System.Text.Json.Serialization;

namespace rEoP.Shared.Model
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MoveType
    {
        Raise,
        Riff,
        ChooseSuit,
        ForceSuit,
        Award
    }

    public class Move
    {
        public MoveType MoveType { get; set; }
        public int Round { get; set; }
        public int Coins { get; set; }
        public Card Card { get; set; }
        public string Player { get; set; }
        public string Comment { get; set; }
    }
}