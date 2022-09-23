using System.Net;

using Painters;

using Unity.Collections;

using UnityEngine;

namespace ArtNet
{
    public class ArtNetClient : MonoBehaviour
    {
        [SerializeField] private StripPainter[] _stripPainters;

        [SerializeField] private string _ipAddress;

        [ReadOnly]
        public string CurrentIP;

        private const int ArtNetPort = 6454;

        private int _stripPixelIndexer;

        private byte[] _dmxData = new byte[512];

        private Color32[] _dmxPixels = new Color32[2720]; // note: Entire subnet of pixels for texture 2D

        private IPAddress _currentIPAddress;

        private UdpCommunicator _communicator;

        private void OnValidate()
        {
            if (IPAddress.TryParse(_ipAddress, out _currentIPAddress))
            {
                CurrentIP = _currentIPAddress.ToString();
                // Todo: Вывести IP в инспектор, чтобы там отображался текущий запаршеный IP
                Debug.Log("IP address set correctly: " + _currentIPAddress);
            }
            else
            {
                Debug.LogError("You should set IP correctly in " + nameof(ArtNetClient));
            }
        }

        private void Start()
        {
            _communicator = new UdpCommunicator();
            _communicator.DataReceived += HandleDataReceiving;
            _communicator.Start(_currentIPAddress, ArtNetPort);
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