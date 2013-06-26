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

        public const UInt32 TYPE_INT = 0;
        public const UInt32 TYPE_LONG = 1;
        public const UInt32 TYPE_STRING = 2;
        public const UInt32 TYPE_FLOAT = 3;

        public const UInt32 ACK_CONFIRM = 1;
        public const UInt32 ACK_ERROR = 0;
        public const UInt32 ORDER_CONFIRM = 2;
    }
}
