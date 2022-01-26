// Decompiled with JetBrains decompiler
// Type: StardewValley.Monsters.ParryEventArgs
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System.IO;

namespace StardewValley.Monsters
{
  internal class ParryEventArgs : NetEventArg
  {
    public int damage;
    private long farmerId;

    public Farmer who
    {
      get => Game1.getFarmer(this.farmerId);
      set => this.farmerId = value.UniqueMultiplayerID;
    }

    public ParryEventArgs()
    {
    }

    public ParryEventArgs(int damage, Farmer who)
    {
      this.damage = damage;
      this.who = who;
    }

    public void Read(BinaryReader reader)
    {
      this.damage = reader.ReadInt32();
      this.farmerId = reader.ReadInt64();
    }

    public void Write(BinaryWriter writer)
    {
      writer.Write(this.damage);
      writer.Write(this.farmerId);
    }
  }
}
