using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.xgame.LoginServer.common.protocol
{
    class ProtocolRequestLogin: IProtocol
    {
        public void Execute(object param)
        {
            Console.WriteLine("ProtocolRequestLogin");
        }
    }
}
