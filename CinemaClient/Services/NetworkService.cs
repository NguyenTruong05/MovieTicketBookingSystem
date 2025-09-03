using System;
using System.Net.Sockets;
using System.Text;

namespace CinemaClient.Services
{
    public class NetworkService
    {
        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private string _currentUser;

        public string CurrentUser { get { return _currentUser; } }

        public bool Connect(string ip, int port)
        {
            try
            {
                _tcpClient = new TcpClient(ip, port);
                _stream = _tcpClient.GetStream();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection error: {ex.Message}");
                return false;
            }
        }

        public string SendMessage(string message)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                _stream.Write(data, 0, data.Length);

                byte[] buffer = new byte[4096];
                int bytesRead = _stream.Read(buffer, 0, buffer.Length);
                return Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }
            catch (Exception ex)
            {
                return $"ERROR|{ex.Message}";
            }
        }

        public void SetCurrentUser(string username)
        {
            _currentUser = username;
        }

        public void Disconnect()
        {
            _stream?.Close();
            _tcpClient?.Close();
        }
    }
}