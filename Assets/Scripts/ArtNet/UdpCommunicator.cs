using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;

namespace ArtNet
{
    public class UdpCommunicator
    {
        public event EventHandler<UdpPacket> DataReceived;

        private UdpClient _socket;

        private BackgroundWorker _server;

        public void Start(IPAddress address, int port)
        {
            _socket = new UdpClient();
            _socket.EnableBroadcast = false;
            _socket.ExclusiveAddressUse = false;
            _socket.Client.SendTimeout = 100;
            _socket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _socket.Client.Bind(new IPEndPoint(address, port));

            _server = new BackgroundWorker();
            _server.DoWork += HandleServerWork;
            _server.RunWorkerAsync();

            _server.RunWorkerCompleted += SubscribeWorker;
        }

        public void Stop()
        {
            _server.RunWorkerCompleted -= SubscribeWorker;
            _server.CancelAsync();
            _socket.Close();
        }

        private void SubscribeWorker(object sender, RunWorkerCompletedEventArgs e)
        {
            _server.RunWorkerAsync();
        }

        private void HandleServerWork(object sender, DoWorkEventArgs e)
        {
            while (!_server.CancellationPending)
            {
                var client = new IPEndPoint(IPAddress.Any, 0);
                var data = _socket.Receive(ref client);

                DataReceived?.Invoke(this, new UdpPacket(client, data));
            }
        }
    }
}