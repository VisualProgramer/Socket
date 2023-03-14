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
        public string PhotoPathmsg { get; set; }
        public DateTime Date { get; set; }


        public MessageModel()
        {
            Date = DateTime.Now;
        }
    }
}
