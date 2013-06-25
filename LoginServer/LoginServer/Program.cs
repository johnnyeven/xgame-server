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
            router.Bind(0x0080, typeof(ProtocolRequestLogin));

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
                    //取得操作数
                    UInt32 packageLength = BitConverter.ToUInt32(new byte[] { receiveData[3], receiveData[2], receiveData[1], receiveData[0] }, 0);
                    UInt16 protocolId = BitConverter.ToUInt16(new byte[] { receiveData[5], receiveData[4] }, 0);
                    int offset = 6;

                    router.triggerProtocol(protocolId, null);
                }
            }
        }
    }
}
