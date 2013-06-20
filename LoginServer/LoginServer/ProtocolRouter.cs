using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using com.xgame.LoginServer.common.protocol;

namespace com.xgame.LoginServer.core.protocol
{
    class ProtocolRouter
    {
        private Dictionary<UInt32, Type> _protocolDictionary;

        public ProtocolRouter()
        {
            _protocolDictionary = new Dictionary<uint, Type>();
        }

        public void Bind(UInt32 protocolId, Type protocol)
        {
            _protocolDictionary[protocolId] = protocol;
        }

        public void UnBind(UInt32 protocolId)
        {
            if (_protocolDictionary.ContainsKey(protocolId))
            {
                _protocolDictionary.Remove(protocolId);
            }
        }

        public bool HasBind(UInt32 protocolId)
        {
            return _protocolDictionary.ContainsKey(protocolId);
        }

        public void triggerProtocol(UInt32 protocolId, Object param)
        {
            if (_protocolDictionary.ContainsKey(protocolId))
            {
                Type protocolType = _protocolDictionary[protocolId];
                object protocolInstance = Activator.CreateInstance(protocolType);
                if (protocolInstance is IProtocol)
                {
                    ((IProtocol)protocolInstance).Execute(param);
                }
            }
        }
    }
}
