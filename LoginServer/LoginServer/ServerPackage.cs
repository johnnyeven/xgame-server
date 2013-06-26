using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace com.xgame.common
{
    class ServerPackage
    {
        public UInt32 success;
        public UInt16 protocolId;
        public ArrayList param;

        public ServerPackage()
        {
            param = new ArrayList();
        }
    }
}
