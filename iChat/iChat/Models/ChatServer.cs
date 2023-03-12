using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace iChat.Models
{
    public class ChatServer
    {
        private int _port = 5555;
        private IPAddress _ipAddress = IPAddress.Parse("10.0.0.52");

        public DataReader dataReader;
        public Socket client;

        public event Action connectedEvent;
        public event Action messageEvent;
        public event Action disconnectedEvent;

        public ChatServer()
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void ConnectToServer(string username, string photoPath)
        {
            if (!client.Connected)
            {
                client.Connect(_ipAddress, _port);
                dataReader = new DataReader(new NetworkStream(client));

                var connectData = new DataCreator();
                connectData.WriteOpcode(0);
                connectData.WriteMessage(username);
                connectData.WriteMessage(photoPath);
                client.Send(connectData.GetData());

                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
        }

        public void SendMessageToServer(string message)
        {
            var messageData = new DataCreator();
            messageData.WriteOpcode(2);
            messageData.WriteMessage(message);
            client.Send(messageData.GetData());
        }

        public void ReadDataFromServer()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var opcode = dataReader.ReadByte();
                    switch (opcode)
                    {
                        case 1:
                            connectedEvent?.Invoke();
                            break;
                        case 2:
                            messageEvent?.Invoke();
                            break;
                        case 3:
                            disconnectedEvent?.Invoke();
                            break;

                        default:
                            break;
                    }
                }
            });
        }
    }
}
