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

            if (guid != UInt32.MinValue && nickName != "")
            {
                MySqlCommand command = new MySqlCommand();
                command.Connection = DatabaseRouter.instance().gameDb();
                command.CommandText = "INSERT INTO game_account(account_guid, nick_name, current_health, max_health, current_mana, max_mana, current_energy, max_energy, current_x, current_y)values";
                command.CommandText += "(" + guid + ", '" + nickName + "', 200, 200, 85, 85, 100, 100, 700, 700)";
                command.ExecuteNonQuery();

                ServerPackage package = new ServerPackage();
                package.success = EnumProtocol.ACK_CONFIRM;
                package.protocolId = 0x0050;

                Int64 accountId = command.LastInsertedId;
                package.param.Add(new Object[] { 8, accountId });

                package.param.Add(new Object[] { nickName.Length, nickName });

                UInt64 accountCash = 0;
                package.param.Add(new Object[] { 8, accountCash });

                UInt32 direction = 0;
                package.param.Add(new Object[] { 4, direction });

                UInt32 currentHealth = 200;
                package.param.Add(new Object[] { 4, currentHealth });

                UInt32 maxHealth = 200;
                package.param.Add(new Object[] { 4, maxHealth });

                UInt32 currentMana = 85;
                package.param.Add(new Object[] { 4, currentMana });

                UInt32 maxMana = 85;
                package.param.Add(new Object[] { 4, maxMana });

                UInt32 currentEnergy = 100;
                package.param.Add(new Object[] { 4, currentEnergy });

                UInt32 maxEnergy = 100;
                package.param.Add(new Object[] { 4, maxEnergy });

                Int32 x = 700;
                package.param.Add(new Object[] { 4, x });

                Int32 y = 700;
                package.param.Add(new Object[] { 4, y });

                CommandCenter.send(vars.client, package);
            }
        }
    }
}
