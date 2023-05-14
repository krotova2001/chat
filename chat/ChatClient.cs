using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace chat
{
    public class ChatClient
    {
        public TcpClient Client=null;
        public String Name="";
        public override string ToString()
        {
            return Name;
        }
    }
}
