using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using com.xgame.common.protocol;

namespace com.xgame.common.protocol
{
    class ProtocolRouter
    {
        private Dictionary<UInt16, Type> _protocolDictionary;

        public ProtocolRouter()
        {
            _protocolDictionary = new Dictionary<UInt16, Type>();
        }

        public void Bind(UInt16 protocolId, Type protocol)
        {
            _protocolDictionary[protocolId] = protocol;
        }

        public void UnBind(UInt16 protocolId)
        {
            if (_protocolDictionary.ContainsKey(protocolId))
            {
                _protocolDictionary.Remove(protocolId);
            }
        }

        public bool HasBind(UInt16 protocolId)
        {
            return _protocolDictionary.ContainsKey(protocolId);
        }

        public void triggerProtocol(UInt16 protocolId, Object param)
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
