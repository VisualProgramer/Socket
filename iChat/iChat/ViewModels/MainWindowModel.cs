using iChat.Commands;
using iChat.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Windows;

namespace iChat.ViewModels
{
    public class MainWindowModel
    {
        private ChatServer _chatServer;

        public iCommand SendMessageToServerCommand { get; set; }
        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<MessageModel> Messages { get; set; }
        public string Username { get; set; }
        public string PhotoPath { get; set; }
        public string Message { get; set; }

        public MainWindowModel(LoginViewModel loginViewModel)
        {
            _chatServer = LoginViewModel.chatServer;
            Username = LoginViewModel.Username;
            PhotoPath = LoginViewModel.PhotoPath;
            _chatServer.ReadDataFromServer();
            Users = new ObservableCollection<UserModel>();
            Messages = new ObservableCollection<MessageModel>();

            _chatServer.connectedEvent += UserConnectedToServer;
            _chatServer.messageEvent += RecievedMessage;
            _chatServer.disconnectedEvent += UserDisconnectedFromServer;

            SendMessageToServerCommand = new iCommand(execute => _chatServer.SendMessageToServer(Message), canExecute => !string.IsNullOrEmpty(Message));
        }

        private void UserDisconnectedFromServer()
        {
            var uid = _chatServer.dataReader.ReadMessage();
            var disconnectedUser = Users.First(x => x.UID == uid);
            Application.Current.Dispatcher.Invoke(() => Users.Remove(disconnectedUser));

            var message = new MessageModel()
            {
                Message = $"{disconnectedUser.Username} disconected",
                PhotoPathmsg = disconnectedUser.PhotoPath
            };
            Application.Current.Dispatcher.Invoke(() => Messages.Add(message));
        }

        private void RecievedMessage()
        {
            var message = new MessageModel();

            message.Message = _chatServer.dataReader.ReadMessage();
            message.PhotoPathmsg = _chatServer.dataReader.ReadMessage();

            Application.Current.Dispatcher.Invoke(() => Messages.Add(message));
        }

        private void UserConnectedToServer()
        {
            var user = new UserModel()
            {
                Username = _chatServer.dataReader.ReadMessage(),
                UID = _chatServer.dataReader.ReadMessage(),
                PhotoPath = _chatServer.dataReader.ReadMessage(),
            };
            if (!Users.Any(x => x.UID == user.UID))
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
            }
        }
    }
}
