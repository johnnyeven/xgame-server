using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;

using com.xgame.common;
using com.xgame.common.database;
using com.xgame.LoginServer.core;
using com.xgame.LoginServer.common;
using com.xgame.common.protocol;

namespace com.xgame.LoginServer.common.protocol
{
    class ProtocolRequestQuickStart: IProtocol
    {
        public void Execute(object param)
        {
            ProtocolParam vars = (ProtocolParam)param;

            int gameId = int.MinValue;
            for (uint i = vars.offset; i < vars.receiveDataLength; )
            {
                uint length = BitConverter.ToUInt32(new byte[] { vars.receiveData[i + 3], vars.receiveData[i + 2], vars.receiveData[i + 1], vars.receiveData[i] }, 0);
                uint type = vars.receiveData[i + 4];
                switch (type)
                {
                    case EnumProtocol.TYPE_INT:
                        if (gameId == int.MinValue)
                        {
                            gameId = BitConverter.ToInt32(new byte[] { vars.receiveData[i + 8], vars.receiveData[i + 7], vars.receiveData[i + 6], vars.receiveData[i + 5] }, 0);
                        }
                        break;
                }
                i += (length + 5);
            }
            Console.WriteLine("[QuickStart] GameId: " + gameId);
            if (gameId != int.MinValue)
            {
                String guid = System.Guid.NewGuid().ToString("N");
                String name = "Guest" + guid;
                String pass = GetMD5(guid);
                Console.WriteLine("[QuickStart] Name: " + name + ", Pass: " + pass);

                if (name != "" && pass != "")
                {
                    MySqlCommand command = new MySqlCommand();
                    command.Connection = DatabaseRouter.instance().platformDb();
                    command.CommandText = "insert into pulse_account(account_name, account_pass) values ('" + name + "', '" + pass + "')";
                    command.ExecuteNonQuery();
                    int insertId = (int)command.LastInsertedId;

                    //String hotkeyConfig = "<root><skill><hotkey code=\"112\" class=\"skill.Skill1\" /><hotkey code=\"113\" class=\"skill.Sheild1\" /></skill></root>";
                    //command.Connection = DatabaseRouter.instance().gameDb();
                    //command.CommandText = "insert into game_hotkey_config values (" + insertId + ", '" + hotkeyConfig + "')";
                    //command.ExecuteNonQuery();
                    /*
                                        $jsonData = Array(
                                                'message'	=>	ACK_SUCCESS,
                                                'user'		=>	$user,
                                                'account_id'=>	$accountId,
                                                'nick_name'	=>	$nickName
                                        );
                     */
                    ServerPackage acksuccess = new ServerPackage();
                    acksuccess.success = EnumProtocol.ACK_CONFIRM;
                    acksuccess.protocolId = 0x0020;
                    acksuccess.param.Add(new object[] { 4, insertId });
                    acksuccess.param.Add(new object[] { name.Length, name });
                    acksuccess.param.Add(new object[] { pass.Length, pass });

                    CommandCenter.send(vars.client, acksuccess);
                }
            }
        }

        public long ConvertDateTimeInt(System.DateTime time)
        {
            //double intResult = 0;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            //intResult = (time- startTime).TotalMilliseconds;
            long t = (time.Ticks - startTime.Ticks) / 10000;            //除10000调整为13位
            return t;
        }

        public string GetMD5(string str)
        {
            string str1 = "";
            byte[] data = Encoding.GetEncoding("utf-8").GetBytes(str);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes = md5.ComputeHash(data);
            for (int i = 0; i < bytes.Length; i++)
            {
                str1 += bytes[i].ToString("x2");
            }
            return str1;
        }
    }
}
