using iChat.Commands;
using iChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace iChat.ViewModels
{
    public class LoginViewModel
    {
        public static ChatServer chatServer = new ChatServer();

        public static string Username { get; set; }
        public static string PhotoPath { get; set; } = @"D:\IT_Step\GitHub\Socket\iChat\iChat\Images\default_user_icon.gif";
        public iCommand ConnectToServerCommand { get; set; }

        public LoginViewModel()
        {
            ConnectToServerCommand = new iCommand(execute => chatServer.ConnectToServer(Username, PhotoPath), canExecute => !string.IsNullOrEmpty(Username));
        }
    }
}
