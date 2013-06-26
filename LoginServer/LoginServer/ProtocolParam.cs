using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace com.xgame.LoginServer.common
{
    class ProtocolParam
    {
        public Socket client;
        public byte[] receiveData;
        public uint receiveDataLength;
        public uint offset;

        public ProtocolParam()
        {
        }
    }
}
