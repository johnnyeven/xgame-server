using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using MySql.Data.MySqlClient;

using com.xgame.LoginServer.core.protocol;
using com.xgame.LoginServer.common.protocol;

namespace com.xgame.LoginServer
{
    class Program
    {
        private static ProtocolRouter router;
        private static MySqlConnection dbConnection;

        static void Main(string[] args)
        {
            router = new ProtocolRouter();
            router.Bind(0x11, typeof(ProtocolRequestLogin));

            String connectionString = "Data Source=localhost;Initial Catalog=pulse_db_platform;User ID=root;Password=84@41%%wi96^4";
            dbConnection = new MySqlConnection(connectionString);
            dbConnection.Open();

            ThreadStart starter = new ThreadStart(Listen);
            Thread listenThread = new Thread(starter);
            listenThread.Start();

            Thread.Sleep(100);

            Console.ReadLine();
        }

        public static void Listen()
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 9040);
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(ipEndPoint);
            server.Listen(10);
            while (true)
            {
                Console.WriteLine("等待客户端登陆连接...");
                Socket client = server.Accept();
                IPEndPoint clientEnpPoint = client.RemoteEndPoint as IPEndPoint;
                Console.WriteLine("客户端已连接, IP:" + clientEnpPoint.Address + ", 端口:" + clientEnpPoint.Port);
                Thread acceptThread = new Thread(new ParameterizedThreadStart(Accept));
                acceptThread.Start(client);
            }
        }

        public static void Accept(object arg)
        {
            Socket client = (Socket)arg;
            int receiveDataLength;
            byte[] receiveData;
            while (true)
            {
                receiveData = new byte[5120];
                receiveDataLength = client.Receive(receiveData);
                if (receiveDataLength == 0)
                {
                    break;
                }

                if (receiveDataLength > 0)
                {
                    int controller, action;

                    //取得操作数
                    controller = receiveData[0] >> 4;
                    action = receiveData[0] & 15;

                    router.triggerProtocol(receiveData[0], null);
                    //if (controller == EnumProtocol.CONTROLLER_INFO)
                    //{
                    //    if (action == EnumProtocol.ACTION_LOGIN)
                    //    {
                    //        //requestLogin(receiveData, receiveDataLength, client);
                    //    }
                    //    else if (action == EnumProtocol.ACTION_REQUEST_CHARACTER)
                    //    {
                    //        //requestCharacter(receiveData, receiveDataLength, client);
                    //    }
                    //    else if (action == EnumProtocol.ACTION_QUICK_START)
                    //    {
                    //        //requestQuickStart(receiveData, receiveDataLength, client);
                    //    }
                    //}
                }
            }
        }
    }
}
