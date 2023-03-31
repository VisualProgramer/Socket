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
        public string PhotoPath { get; set; }
        public Guid UID { get; set; }
        public TcpClient clientSocket { get; set; }

        private DataReader _dataReader;
        private byte opcode;

        public ChatUser(TcpClient client)
        {
            clientSocket = client;
            UID = Guid.NewGuid();

            _dataReader = new DataReader(clientSocket.GetStream());
            opcode = _dataReader.ReadOpcode();
            if (opcode == 0)
            {
                Username = _dataReader.ReadMessage();
                PhotoPath = _dataReader.ReadMessage();
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
                    var opcode = _dataReader.ReadOpcode();
                    switch (opcode)
                    {
                        case 2:
                            var message = _dataReader.ReadMessage();
                            Console.WriteLine($"[{DateTime.Now}] {Username}: {message}");
                            Program.SendMessageToUsers(message, PhotoPath);
                            break;

                        case 4:
                            var privateMessage = _dataReader.ReadMessage();
                            var userUid = _dataReader.ReadMessage();
                            Console.WriteLine($"[{DateTime.Now}] {Username}: {privateMessage} to {Program._users.First(x=>x.UID.ToString()==userUid).Username}");
                            Program.SendPrivateMessage(privateMessage, userUid, UID.ToString());
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
