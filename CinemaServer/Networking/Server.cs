using System;
using System.Net;
using System.Net.Sockets;

namespace CinemaServer.Networking
{
    public class Server
    {
        private TcpListener _tcpListener;
        private bool _isRunning;
        private int _port;

        public Server(int port)
        {
            _port = port;
            _tcpListener = new TcpListener(IPAddress.Any, _port);
        }

        public void Start()
        {
            try
            {
                _isRunning = true;
                _tcpListener.Start();
                Console.WriteLine($"Server started on port {_port}...");

                while (_isRunning)
                {
                    TcpClient client = _tcpListener.AcceptTcpClient();
                    Console.WriteLine($"Client connected: {client.Client.RemoteEndPoint}");

                    ClientHandler clientHandler = new ClientHandler(client);
                    System.Threading.Thread clientThread = new System.Threading.Thread(clientHandler.HandleClient);
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Server error: {ex.Message}");
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _tcpListener?.Stop();
        }
    }
}