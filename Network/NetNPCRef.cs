// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetNPCRef
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System;

namespace StardewValley.Network
{
  public class NetNPCRef : INetObject<NetFields>
  {
    private readonly NetGuid guid = new NetGuid();

    public NetFields NetFields { get; } = new NetFields();

    public NetNPCRef() => this.NetFields.AddFields((INetSerializable) this.guid);

    public NPC Get(GameLocation location)
    {
      if (this.guid.Value == Guid.Empty || location == null)
        return (NPC) null;
      return !location.characters.ContainsGuid(this.guid.Value) ? (NPC) null : location.characters[this.guid.Value];
    }

    public void Set(GameLocation location, NPC npc)
    {
      if (npc == null)
      {
        this.guid.Value = Guid.Empty;
      }
      else
      {
        Guid guid = location.characters.GuidOf(npc);
        this.guid.Value = !(guid == Guid.Empty) ? guid : throw new ArgumentException();
      }
    }

    public void Clear() => this.guid.Value = Guid.Empty;
  }
}
