// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetBuildingRef
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using StardewValley.Buildings;
using System.Collections;
using System.Collections.Generic;

namespace StardewValley.Network
{
  public class NetBuildingRef : INetObject<NetFields>, IEnumerable<Building>, IEnumerable
  {
    private readonly NetString nameOfIndoors = new NetString();

    public NetFields NetFields { get; } = new NetFields();

    public Building Value
    {
      get
      {
        string name = this.nameOfIndoors.Get();
        return name == null ? (Building) null : Game1.getFarm().getBuildingByName(name);
      }
      set
      {
        if (value == null)
          this.nameOfIndoors.Value = (string) null;
        else
          this.nameOfIndoors.Value = value.nameOfIndoors;
      }
    }

    public NetBuildingRef() => this.NetFields.AddFields((INetSerializable) this.nameOfIndoors);

    public IEnumerator<Building> GetEnumerator()
    {
      yield return this.Value;
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public static implicit operator Building(NetBuildingRef buildingRef) => buildingRef.Value;
  }
}
