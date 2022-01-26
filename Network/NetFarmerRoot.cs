// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetFarmerRoot
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;

namespace StardewValley.Network
{
  public class NetFarmerRoot : NetRoot<Farmer>
  {
    public NetFarmerRoot() => this.Serializer = SaveGame.farmerSerializer;

    public NetFarmerRoot(Farmer value)
      : base(value)
    {
      this.Serializer = SaveGame.farmerSerializer;
    }

    public override NetRoot<Farmer> Clone()
    {
      NetRoot<Farmer> netRoot = base.Clone();
      if ((NetFieldBase<Farmer, NetRef<Farmer>>) Game1.serverHost != (NetRef<Farmer>) null && netRoot.Value != null)
        netRoot.Value.teamRoot = Game1.serverHost.Value.teamRoot;
      return netRoot;
    }
  }
}
