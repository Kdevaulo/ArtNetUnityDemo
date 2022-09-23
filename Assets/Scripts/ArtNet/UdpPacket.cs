using System.Net;

namespace ArtNet
{
    public class UdpPacket
    {
        public IPEndPoint EndPoint { get; }

        public byte[] RawData { get; }

        public int Universe => RawData[14] % 16;

        public int Net => RawData[15];

        public int SubNet => RawData[14] / 16;

        public UdpPacket(IPEndPoint endPoint, byte[] data)
        {
            EndPoint = endPoint;
            RawData = data;
        }
    }
}