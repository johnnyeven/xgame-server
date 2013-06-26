using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.xgame.GameServer.common.protocol
{
    class EnumProtocol
    {
        public const UInt32 CONTROLLER_BATTLE = 3;
        public const UInt32 CONTROLLER_MSG = 2;
        public const UInt32 CONTROLLER_MOVE = 1;
        public const UInt32 CONTROLLER_INFO = 0;
        public const UInt32 NPC_CONTROLLER_BATTLE = 13;
        public const UInt32 NPC_CONTROLLER_MOVE = 11;
        /*
         * Action
         */
        //MOVE
        public const UInt32 ACTION_MOVETO = 0;
        public const UInt32 ACTION_MOVE = 1;
        //MSG
        public const UInt32 ACTION_PUBLIC_MSG = 0;
        public const UInt32 ACTION_PRIVATE_MSG = 1;
        //BATTLE
        public const UInt32 ACTION_ATTACK = 0;
        public const UInt32 ACTION_PREPARE_ATTACK = 2;
        public const UInt32 ACTION_UNDERATTACK = 1;
        public const UInt32 ACTION_SING = 3;
        //INFO
        public const UInt32 ACTION_INIT_INFO = 0;
        public const UInt32 ACTION_CAMERAVIEW_OBJECT_LIST = 4;
        public const UInt32 ACTION_CHANGE_ACTION = 3;
        public const UInt32 ACTION_LOGIN = 1;
        public const UInt32 ACTION_LOGOUT = 2;
        public const UInt32 ACTION_INIT_CHARACTER = 6;
        public const UInt32 ACTION_CHANGE_DIRECTION = 7;

        public const UInt32 ACK_CONFIRM = 1;
        public const UInt32 ACK_ERROR = 0;
        public const UInt32 ORDER_CONFIRM = 2;

        public const UInt32 TYPE_UInt32 = 0;
        public const UInt32 TYPE_LONG = 1;
        public const UInt32 TYPE_STRING = 2;
        public const UInt32 TYPE_FLOAT = 3;
        public const UInt32 TYPE_BOOL = 4;
    }
}
