
using System.Net;
using System.Net.Sockets;

namespace iServer
{
    class Program
    {
        private static Socket _serverSocket;
        private static List<ChatUser> _users;
        private static int port = 5555;
        private static IPAddress ipAddress = IPAddress.Parse("10.0.0.52");
        private static IPEndPoint _endPoint = new IPEndPoint(ipAddress, port);

        static void Main(string[] args)
        {
            _users = new List<ChatUser>();
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverSocket.Bind(_endPoint);
            _serverSocket.Listen(10);

            while (true)
            {
                var user = new ChatUser(_serverSocket.Accept());
                _users.Add(user);

                SendAllUsersToUsers();
            }
        }

        private static void SendAllUsersToUsers()
        {
            foreach (var sentToUser in _users)
            {
                foreach (var user in _users)
                {
                    if (sentToUser.UID != user.UID)
                    {
                        var usersData = new DataCreator();
                        usersData.WriteOpcode(1);
                        usersData.WriteMessage(user.Username);
                        usersData.WriteMessage(user.UID.ToString());
                        usersData.WriteMessage(user.PhotoPath);
                        sentToUser.clientSocket.Send(usersData.GetData());
                    }
                }
            }
        }

        public static void SendMessageToUsers(string message, string photoPath)
        {
            foreach (var user in _users)
            {
                var messageData = new DataCreator();
                messageData.WriteOpcode(2);
                messageData.WriteMessage(message);
                messageData.WriteMessage(photoPath);
                user.clientSocket.Send(messageData.GetData());
            }
        }

        public static void SendMessageToUsers(string message)
        {
            foreach (var user in _users)
            {
                var messageData = new DataCreator();
                messageData.WriteOpcode(2);
                messageData.WriteMessage(message);
                user.clientSocket.Send(messageData.GetData());
            }
        }


        public static void SendAllUsersDisconnectedUser(string uid)
        {
            var disconnectedUser = _users.First(x => x.UID.ToString() == uid);
            _users.Remove(disconnectedUser);

            foreach (var user in _users)
            {                
                var disconnectData = new DataCreator();
                disconnectData.WriteOpcode(3);
                disconnectData.WriteMessage(uid);
                user.clientSocket.Send(disconnectData.GetData());
            }
        }
    }
}