// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetCharacterRef
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System;

namespace StardewValley.Network
{
  public class NetCharacterRef : INetObject<NetFields>
  {
    private readonly NetNPCRef npc = new NetNPCRef();
    private readonly NetFarmerRef farmer = new NetFarmerRef();

    public NetFields NetFields { get; } = new NetFields();

    public NetCharacterRef() => this.NetFields.AddFields((INetSerializable) this.npc.NetFields, (INetSerializable) this.farmer.NetFields);

    public Character Get(GameLocation location) => (Character) this.npc.Get(location) ?? (Character) this.farmer.Value;

    public void Set(GameLocation location, Character character)
    {
      switch (character)
      {
        case NPC _:
          this.npc.Set(location, character as NPC);
          this.farmer.Value = (Farmer) null;
          break;
        case Farmer _:
          this.npc.Clear();
          this.farmer.Value = character as Farmer;
          break;
        default:
          throw new ArgumentException();
      }
    }

    public void Clear()
    {
      this.npc.Clear();
      this.farmer.Value = (Farmer) null;
    }
  }
}
