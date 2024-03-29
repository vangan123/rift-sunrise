﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace Common
{
    [Serializable]
    [ISerializableAttribute((long)Opcodes.ProtocolHandshakeCompression)]
    public class HandshakeCompression : ISerializablePacket
    {
        [BoolBitAttribute(0)]
        public bool Enabled;

        public override void OnRead(RiftClient From)
        {
            Log.Success("HanshakeCompression", "Client Compression : " + Enabled);
            From.EnableReceiveCompress();
        }
    }
}
