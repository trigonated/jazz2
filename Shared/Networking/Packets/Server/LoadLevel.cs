﻿using Jazz2.Game;
using Lidgren.Network;

namespace Jazz2.Networking.Packets.Server
{
    public struct LoadLevel : IServerPacket
    {
        public NetConnection SenderConnection { get; set; }

        byte IServerPacket.Type => 11;


        public string ServerName;
        public string LevelName;
        public MultiplayerLevelType LevelType;
        public byte AssignedPlayerIndex;

        void IServerPacket.Read(NetIncomingMessage msg)
        {
            ServerName = msg.ReadString();
            LevelName = msg.ReadString();
            LevelType = (MultiplayerLevelType)msg.ReadByte();
            AssignedPlayerIndex = msg.ReadByte();
        }

        void IServerPacket.Write(NetOutgoingMessage msg)
        {
            msg.Write(ServerName);
            msg.Write(LevelName);
            msg.Write((byte)LevelType);
            msg.Write((byte)AssignedPlayerIndex);
        }
    }
}