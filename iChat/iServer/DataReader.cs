using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace iServer
{
    public class DataReader : BinaryReader
    {
        private NetworkStream _ns;
        public DataReader(NetworkStream ns) : base(ns)
        {
            _ns = ns;  
        }
        public int ReadOpcode()
        {
            return _ns.ReadByte();
        }

        public string ReadMessage()
        {
            var lengthMessage = ReadInt32();
            byte[] buffer = new byte[lengthMessage];
            _ns.Read(buffer, 0, lengthMessage);

            return Encoding.ASCII.GetString(buffer);
        }
    }
}
