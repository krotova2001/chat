using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    [Serializable]
    public class ChatClient
    {
        public TcpClient Client { get; set; }
        public String Name { get; set; }  = "";
        public override string ToString()
        {
            return Name;
        }
    }
}
