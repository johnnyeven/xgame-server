﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.xgame.LoginServer.common.protocol
{
    interface IProtocol
    {
        void Execute(Object param);
    }
}
