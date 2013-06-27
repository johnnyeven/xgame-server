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
    class ProtocolRequestAccountRole: IProtocol
    {
        public void Execute(object param)
        {
            ProtocolParam vars = (ProtocolParam)param;

            uint guid = uint.MinValue;
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
                }
                i += (length + 5);
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[RequestAccountRole] GUID: " + guid);

            if (guid != uint.MinValue)
            {
                MySqlCommand command = new MySqlCommand();
                command.Connection = DatabaseRouter.instance().gameDb();
                command.CommandText = "SELECT *  FROM `game_account` WHERE `account_guid` = " + guid;

                MySqlDataReader result = command.ExecuteReader();
                ServerPackage package = new ServerPackage();
                package.success = EnumProtocol.ACK_CONFIRM;
                package.protocolId = 0x0040;
                if (result.HasRows)
                {
                    if (result.Read())
                    {
                        Int64 accountId = result.GetInt64("account_id");
                        package.param.Add(new Object[] {8, accountId});

                        String nickName = result.GetString("nick_name");
                        package.param.Add(new Object[] {nickName.Length, nickName});

                        UInt64 accountCash = result.GetUInt64("account_cash");
                        package.param.Add(new Object[] { 8, accountCash });

                        Int32 direction = result.GetInt32("direction");
                        package.param.Add(new Object[] { 4, direction });

                        Int32 currentHealth = result.GetInt32("current_health");
                        package.param.Add(new Object[] { 4, currentHealth });

                        Int32 maxHealth = result.GetInt32("max_health");
                        package.param.Add(new Object[] { 4, maxHealth });

                        Int32 currentMana = result.GetInt32("current_mana");
                        package.param.Add(new Object[] { 4, currentMana });

                        Int32 maxMana = result.GetInt32("max_mana");
                        package.param.Add(new Object[] { 4, maxMana });

                        Int32 currentEnergy = result.GetInt32("current_energy");
                        package.param.Add(new Object[] { 4, currentEnergy });

                        Int32 maxEnergy = result.GetInt32("max_energy");
                        package.param.Add(new Object[] { 4, maxEnergy });

                        Int32 x = result.GetInt32("current_x");
                        package.param.Add(new Object[] { 4, x });

                        Int32 y = result.GetInt32("current_y");
                        package.param.Add(new Object[] { 4, y });
                    }
                }
                else
                {
                    package.param.Add(new Object[] { 8, (Int64)(-1) });
                }
                result.Close();
                
                CommandCenter.send(vars.client, package);
            }
        }
    }
}
