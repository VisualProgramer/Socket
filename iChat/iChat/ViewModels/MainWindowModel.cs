using iChat.Commands;
using iChat.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace iChat.ViewModels
{
    public class MainWindowModel
    {
        private ChatServer _chatServer;

        public iCommand ConnectToServerCommand { get; set; }
        public iCommand SendMessageToServerCommand { get; set; }
        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }

        public MainWindowModel()
        {
            _chatServer = new ChatServer();
            Users = new ObservableCollection<UserModel>();
            Messages = new ObservableCollection<string>();

            _chatServer.connectedEvent += UserConnectedToServer;
            _chatServer.messageEvent += RecievedMessage;
            _chatServer.disconnectedEvent += UserDisconnectedFromServer;

            ConnectToServerCommand = new iCommand(execute => _chatServer.ConnectToServer(Username), canExecute => !string.IsNullOrEmpty(Username));
            SendMessageToServerCommand = new iCommand(execute => _chatServer.SendMessageToServer(Message), canExecute => !string.IsNullOrEmpty(Message));
        }

        private void UserDisconnectedFromServer()
        {
            var uid = _chatServer.dataReader.ReadMessage();
            var disconnectedUser = Users.First(x => x.UID == uid);
            Application.Current.Dispatcher.Invoke(()=> Users.Remove(disconnectedUser));
        }

        private void RecievedMessage()
        {
            var message = _chatServer.dataReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() => Messages.Add(message));
        }

        private void UserConnectedToServer()
        {
            var user = new UserModel()
            {
                Username = _chatServer.dataReader.ReadMessage(),
                UID = _chatServer.dataReader.ReadMessage()
            };

            if (!Users.Any(x => x.UID == user.UID))
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
            }
        }
    }
}
