
using System.Net;
using System.Net.Sockets;

namespace iServer
{
    class Program
    {
        private static TcpListener _tcpListener;
        private static List<ChatUser> _users;
        private static int port = 5555;
        private static IPAddress ipAddress = IPAddress.Parse("10.0.0.52");

        static void Main(string[] args)
        {
            _users = new List<ChatUser>();
            _tcpListener = new TcpListener(ipAddress, port);
            _tcpListener.Start();

            while (true)
            {
                var user = new ChatUser(_tcpListener.AcceptTcpClient());
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
                    var usersData = new DataCreator();
                    usersData.WriteOpcode(1);
                    usersData.WriteMessage(user.Username);
                    usersData.WriteMessage(user.UID.ToString());
                    sentToUser.clientSocket.Client.Send(usersData.GetData());
                }
            }
        }

        public static void SendMessageToUsers(string message)
        {
            foreach (var user in _users)
            {
                var messageData = new DataCreator();
                messageData.WriteOpcode(2);
                messageData.WriteMessage(message);
                user.clientSocket.Client.Send(messageData.GetData());
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
                user.clientSocket.Client.Send(disconnectData.GetData());
            }

            SendMessageToUsers($"{disconnectedUser.Username} disconnected");
        }
    }
}