using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public struct PlayerData:IEquatable<PlayerData>,INetworkSerializable
{

    public ulong clientId;
    public TeamType teamType;

    public bool Equals(PlayerData other)
    {
        return clientId == other.clientId;
        
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref teamType);
    }
}
