using System.Net;

using Models;

using Painters;

using UnityEngine;

namespace ArtNet
{
    public class ArtNetClient : MonoBehaviour
    {
        [SerializeField] private StripPainter[] _stripPainters;

        [SerializeField] private CustomIPAddress _customIPAddress;

        private const int ArtNetPort = 6454;

        private int _stripPixelIndexer;

        private byte[] _dmxData = new byte[512];

        private Color32[] _dmxPixels = new Color32[2720]; // note: Entire subnet of pixels for texture 2D

        private UdpCommunicator _communicator;

        private void OnValidate()
        {
            IPAddress.TryParse(_customIPAddress.CurrentIP, out _customIPAddress.IPAddress);

            _customIPAddress.IPLabel = _customIPAddress.IPAddress is null
                ? "Ip Address Is Not Set"
                : _customIPAddress.IPAddress.ToString();
        }

        private void Start()
        {
            _communicator = new UdpCommunicator();
            _communicator.DataReceived += HandleDataReceiving;
            _communicator.Start(_customIPAddress.IPAddress, ArtNetPort);
        }

        private void Update()
        {
            foreach (var painter in _stripPainters)
            {
                painter.FillStrip(GetColors(painter.DiodsCount));
            }

            _stripPixelIndexer = 0;
        }

        /// <summary>
        /// Call this to stop communicator
        /// </summary>
        public void Stop()
        {
            _communicator.Stop();
        }

        private Color32[] GetColors(int colorsCount)
        {
            var colors = new Color32[colorsCount];

            for (int i = 0; i < colorsCount; i++)
            {
                colors[i] = _dmxPixels[_stripPixelIndexer + i];
            }

            _stripPixelIndexer += colorsCount;

            return colors;
        }

        private void HandleDataReceiving(object sender, UdpPacket e)
        {
            var packet = new ArtNetPacket(e.EndPoint, e.RawData);

            if (packet.IsValid)
            {
                var universe = packet.Universe;

                for (int i = 0; i < 512; i++)
                {
                    _dmxData[i] = packet.RawData[18 + i];
                }

                for (int i = 0; i < 170; i++)
                {
                    _dmxPixels[universe * 170 + i] =
                        new Color32(_dmxData[i * 3], _dmxData[i * 3 + 1], _dmxData[i * 3 + 2], 255);
                }
            }
        }
    }
}