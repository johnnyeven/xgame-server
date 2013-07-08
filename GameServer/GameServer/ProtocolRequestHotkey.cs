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
    class ProtocolRequestHotkey: IProtocol
    {
        public void Execute(object param)
        {
            ProtocolParam vars = (ProtocolParam)param;

            Int64 accountId = Int64.MinValue;
            for (uint i = vars.offset; i < vars.receiveDataLength; )
            {
                uint length = BitConverter.ToUInt32(new byte[] { vars.receiveData[i + 3], vars.receiveData[i + 2], vars.receiveData[i + 1], vars.receiveData[i] }, 0);
                uint type = vars.receiveData[i + 4];
                switch (type)
                {
                    case EnumProtocol.TYPE_LONG:
                        if (accountId == Int64.MinValue)
                        {
                            accountId = BitConverter.ToInt64(new byte[] { vars.receiveData[i + 12], vars.receiveData[i + 11], vars.receiveData[i + 10], vars.receiveData[i + 9], vars.receiveData[i + 8], vars.receiveData[i + 7], vars.receiveData[i + 6], vars.receiveData[i + 5] }, 0);
                        }
                        break;
                }
                i += (length + 5);
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[RequestHotkey] AccountId: " + accountId);

            if (accountId != Int64.MinValue)
            {
                MySqlCommand command = new MySqlCommand();
                command.Connection = DatabaseRouter.instance().gameDb();
                command.CommandText = "SELECT *  FROM `game_hotkey_config` WHERE `account_id` = " + accountId;

                MySqlDataReader result = command.ExecuteReader();
                ServerPackage package = new ServerPackage();
                package.success = EnumProtocol.ACK_CONFIRM;
                package.protocolId = 0x0060;
                if (result.HasRows)
                {
                    if (result.Read())
                    {
                        //Int64 accountId = result.GetInt64("account_id");
                        //package.param.Add(new Object[] {8, accountId});
                    }
                }
                else
                {
                    //package.param.Add(new Object[] { 8, (Int64)(-1) });
                }
                result.Close();
                
                CommandCenter.send(vars.client, package);
            }
        }
    }
}
