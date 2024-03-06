using ProtoBuf;

namespace RIN.InternalAPI.Models
{
    [ProtoContract]
    public class Event
    {
        [ProtoMember(1)] public string Type { get; set; }
        [ProtoMember(2)] public string Payload { get; set; }
    }
}
