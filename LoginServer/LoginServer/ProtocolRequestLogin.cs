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
    class ProtocolRequestLogin: IProtocol
    {
        public void Execute(object param)
        {
            ProtocolParam vars = (ProtocolParam)param;
            IPEndPoint clientEnpPoint = vars.client.RemoteEndPoint as IPEndPoint;
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
                int serverId = int.MinValue;
                Console.WriteLine("[QuickStart] Name: " + name + ", Pass: " + pass);

                MySqlCommand command = new MySqlCommand();
                command.Connection = DatabaseRouter.instance().gameDb();
                command.CommandText = "select * from game_server where game_id=" + gameId + " and server_recommend=1";
                MySqlDataReader serverResult = command.ExecuteReader();
                if (serverResult.HasRows)
                {
                    serverResult.Read();
                    serverId = serverResult.GetInt32("account_server_id");
                }
                else
                {
                    serverResult.Close();
                    command.CommandText = "select * from game_server where game_id=" + gameId + " order by account_count desc";
                    serverResult = command.ExecuteReader();
                    if (serverResult.Read())
                    {
                        serverId = serverResult.GetInt32("account_server_id");
                    }
                }
                serverResult.Close();
                serverResult.Dispose();

                if (name != "" && pass != "")
                {
                    command.Connection = DatabaseRouter.instance().platformDb();
                    command.CommandText = "insert into pulse_account(account_name, account_pass) values ('" + name + "', '" + pass + "')";
                    command.ExecuteNonQuery();
                    int insertId = (int)command.LastInsertedId;
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
                    acksuccess.protocolId = 0x0080;
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
