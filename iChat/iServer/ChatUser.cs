using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace iServer
{
    public class ChatUser
    {
        public string Username { get; set; }
        public Guid UID { get; set; }
        public TcpClient clientSocket { get; set; }

        private DataReader _dataReader;

        public ChatUser(TcpClient client)
        {
            clientSocket = client;
            UID = Guid.NewGuid();

            _dataReader = new DataReader(clientSocket.GetStream());
            var opcode = _dataReader.ReadByte();
            if (opcode == 0)
            {
                Username = _dataReader.ReadMessage();
            }

            Console.WriteLine($"User \"{Username}\" connected");

            Task.Run(() => ProcessingAllGetingData());
        }

        
        public void ProcessingAllGetingData()
        {
            while (true)
            {
                try
                {
                    var opcode = _dataReader.ReadByte();
                    switch (opcode)
                    {
                        case 2:
                            var message = _dataReader.ReadMessage();
                            Console.WriteLine($"[{DateTime.Now}] {Username}: {message}");
                            Program.SendMessageToUsers(message);
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine($"{Username}: Disconnected");
                    Program.SendAllUsersDisconnectedUser(UID.ToString());
                    clientSocket.Close();
                    break;
                }
            }
        }
    }
}
