using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.xgame.LoginServer.common.protocol
{
    class EnumProtocol
    {
        public const UInt32 CONTROLLER_INFO = 0;
        public const UInt32 ACTION_LOGIN = 1;
        public const UInt32 ACTION_REQUEST_CHARACTER = 2;
        public const UInt32 ACTION_QUICK_START = 8;
    }
}
