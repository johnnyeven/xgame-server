using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using com.xgame.common.protocol;

namespace com.xgame.GameServer.common.protocol
{
    class ProtocolRequestInitInfo: IProtocol
    {
        public void Execute(object param)
        {
            ProtocolParam vars = (ProtocolParam)param;
        }
    }
}
