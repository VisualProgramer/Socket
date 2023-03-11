using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iServer
{
    internal class DataCreator
    {
        private MemoryStream _ms;

        public DataCreator()
        {
            _ms = new MemoryStream();
        }

        public void WriteOpcode(byte opcode)
        {
            _ms.WriteByte(opcode);
        }

        public void WriteMessage(string message)
        {
            _ms.Write(BitConverter.GetBytes(message.Length));
            _ms.Write(Encoding.ASCII.GetBytes(message));
        }

        public byte[] GetData()
        {
            return _ms.ToArray();
        }
    }
}
