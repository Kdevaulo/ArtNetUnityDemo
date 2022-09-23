using System.Linq;
using System.Net;

namespace ArtNet
{
    public class ArtNetPacket : UdpPacket
    {
        public bool IsValid => _artNetIdentifier.SequenceEqual(RawData.Block(0, _artNetIdentifier.Length));

        private readonly byte[] _artNetIdentifier = {0x41, 0x72, 0x74, 0x2d, 0x4e, 0x65, 0x74, 0};

        public ArtNetPacket(IPEndPoint endPoint, byte[] rawData) : base(endPoint, rawData)
        {
        }
    }
}