using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

using com.xgame.common;
using com.xgame.common.protocol;
using com.xgame.common.database;
using com.xgame.GameServer.core;

namespace com.xgame.GameServer.common.protocol
{
    class ProtocolRegisterAccountRole: IProtocol
    {
        public void Execute(object param)
        {
            ProtocolParam vars = (ProtocolParam)param;

            UInt32 guid = UInt32.MinValue;
            String nickName = "";

            for (uint i = vars.offset; i < vars.receiveDataLength; )
            {
                uint length = BitConverter.ToUInt32(new byte[] { vars.receiveData[i + 3], vars.receiveData[i + 2], vars.receiveData[i + 1], vars.receiveData[i] }, 0);
                uint type = vars.receiveData[i + 4];
                switch (type)
                {
                    case EnumProtocol.TYPE_INT:
                        if (guid == uint.MinValue)
                        {
                            guid = BitConverter.ToUInt32(new byte[] { vars.receiveData[i + 8], vars.receiveData[i + 7], vars.receiveData[i + 6], vars.receiveData[i + 5] }, 0);
                        }
                        break;
                    case EnumProtocol.TYPE_STRING:
                        if (nickName == "")
                        {
                            nickName = Encoding.UTF8.GetString(vars.receiveData, (int)i + 5, (int)length);
                        }
                        break;
                }
                i += (length + 5);
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[RegisterAccountRole] GUID: " + guid + ", NickName: " + nickName);
        }
    }
}
