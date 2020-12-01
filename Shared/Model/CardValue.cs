using System.Text.Json.Serialization;

namespace rEoP.Shared.Model
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Value
    {
        // explicit to highlight comparison rules
        // underscore to keep it uniform.
        _2 = 1,
        _3 = 2,
        _4 = 3,
        _5 = 4,
        _6 = 5,
        _7 = 6,
        _8 = 7,
        _9 = 8,
        _10 = 9,
        _J = 10,
        _Q = 11,
        _K = 12,
        _A = 13
    }
}
