using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

using com.xgame.common;
using com.xgame.GameServer.common;
using com.xgame.GameServer.common.protocol;

namespace com.xgame.GameServer.core
{
    class CommandCenter
    {
        public static void send(Socket socket, ServerPackage package)
        {
            int dataLength = 0;

            byte[] result = new byte[1024];
            result[4] = (byte)(package.protocolId & 0x00FF);
            result[5] = (byte)(package.protocolId >> 8);
            result[6] = (byte)package.success;
            dataLength += 3;

            int resultOffset = 7;
            for (int i = 0; i < package.param.Count; i++)
            {
                object[] parameter = (object[])package.param[i];

                int length = (int)parameter[0];
                byte[] lengthBytes = BitConverter.GetBytes(length);
                lengthBytes.CopyTo(result, resultOffset);
                dataLength += 4;
                resultOffset += 4;

                if (parameter[1].GetType() == typeof(String))
                {
                    result[resultOffset] = (byte)EnumProtocol.TYPE_STRING;
                    dataLength += 1;
                    resultOffset += 1;

                    byte[] bytes = Encoding.UTF8.GetBytes((String)parameter[1]);
                    bytes.CopyTo(result, resultOffset);
                    dataLength += length;
                    resultOffset += length;
                }
                else if (parameter[1].GetType() == typeof(int))
                {
                    result[resultOffset] = (byte)EnumProtocol.TYPE_INT;
                    dataLength += 1;
                    resultOffset += 1;

                    byte[] bytes = BitConverter.GetBytes((int)parameter[1]);
                    bytes.CopyTo(result, resultOffset);
                    dataLength += 4;
                    resultOffset += 4;
                }
                else if (parameter[1].GetType() == typeof(float))
                {
                    result[resultOffset] = (byte)EnumProtocol.TYPE_FLOAT;
                    dataLength += 1;
                    resultOffset += 1;
                    byte[] bytes = BitConverter.GetBytes((float)parameter[1]);
                    bytes.CopyTo(result, resultOffset);
                    dataLength += 4;
                    resultOffset += 4;
                }
                else if (parameter[1].GetType() == typeof(long))
                {
                    result[resultOffset] = (byte)EnumProtocol.TYPE_LONG;
                    dataLength += 1;
                    resultOffset += 1;
                    byte[] bytes = BitConverter.GetBytes((long)parameter[1]);
                    bytes.CopyTo(result, resultOffset);
                    dataLength += 8;
                    resultOffset += 8;
                }
            }
            byte[] packageLength = BitConverter.GetBytes(dataLength);
            packageLength.CopyTo(result, 0);
            dataLength += 4;
            socket.Send(result, dataLength, SocketFlags.None);
        }
    }
}
