using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iChat.ViewModels
{
    public class MessageModel
    {
        public string Message { get; set; }
        public string PhotoPath { get; set; }
        public DateTime Date { get; set; }


        public MessageModel()
        {
            Date = DateTime.Now;
        }
    }
}
