using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

using com.xgame.common.protocol;
using com.xgame.common.database;
using com.xgame.GameServer.common;
using com.xgame.GameServer.common.protocol;

namespace com.xgame.GameServer
{
    class Program
    {
        private static ProtocolRouter router;

        [STAThread]
        static void Main(string[] args)
        {
            router = new ProtocolRouter();
            DatabaseRouter.instance();

            router.Bind(0x0040, typeof(ProtocolRequestAccountRole));
            router.Bind(0x0050, typeof(ProtocolRegisterAccountRole));
            router.Bind(0x0060, typeof(ProtocolRequestHotkey));

            ThreadStart starter = new ThreadStart(Listen);
            Thread listenThread = new Thread(starter);
            listenThread.Start();

            Thread.Sleep(100);

            Console.ReadLine();
        }

        public static void Listen()
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 9050);
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(ipEndPoint);
            server.Listen(10);
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("等待客户端连接...");
                Socket client = server.Accept();
                IPEndPoint clientEnpPoint = client.RemoteEndPoint as IPEndPoint;
                Console.ForegroundColor = ConsoleColor.Red;
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
                try
                {
                    receiveDataLength = client.Receive(receiveData);
                }
                catch (SocketException err)
                {
                    Console.WriteLine(err.Message);
                    return;
                }
                if (receiveDataLength == 0)
                {
                    break;
                }

                if (receiveDataLength > 0)
                {
                    //取得操作数
                    UInt32 packageLength = BitConverter.ToUInt32(new byte[] { receiveData[3], receiveData[2], receiveData[1], receiveData[0] }, 0);
                    UInt16 protocolId = BitConverter.ToUInt16(new byte[] { receiveData[5], receiveData[4] }, 0);

                    ProtocolParam param = new ProtocolParam();
                    param.client = client;
                    param.receiveData = receiveData;
                    param.receiveDataLength = (uint)receiveDataLength;
                    param.offset = 6;

                    router.triggerProtocol(protocolId, param);
                }
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("客户端已断开, IP:" + ((IPEndPoint)client.RemoteEndPoint).Address + ", 端口:" + ((IPEndPoint)client.RemoteEndPoint).Port);
        }
    }
}
