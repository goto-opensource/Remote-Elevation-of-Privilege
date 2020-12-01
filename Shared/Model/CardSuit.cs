using System.Text.Json.Serialization;

namespace rEoP.Shared.Model
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Suit
    {
        Spoofing, Tampering, Repudiation, InformationDisclosure, DenialOfService, ElevationOfPrivilege, Privacy
    }
}
