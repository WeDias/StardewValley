// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetFarmerRef
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System.Collections;
using System.Collections.Generic;

namespace StardewValley.Network
{
  public class NetFarmerRef : INetObject<NetFields>, IEnumerable<Farmer>, IEnumerable
  {
    private readonly NetBool defined = new NetBool();
    private readonly NetLong uid = new NetLong();

    public NetFields NetFields { get; } = new NetFields();

    public long UID
    {
      get => !(bool) (NetFieldBase<bool, NetBool>) this.defined ? 0L : this.uid.Value;
      set
      {
        this.uid.Value = value;
        this.defined.Value = true;
      }
    }

    public Farmer Value
    {
      get => !(bool) (NetFieldBase<bool, NetBool>) this.defined ? (Farmer) null : this.getFarmer((long) this.uid);
      set
      {
        this.defined.Value = value != null;
        this.uid.Value = value != null ? value.UniqueMultiplayerID : 0L;
      }
    }

    public NetFarmerRef() => this.NetFields.AddFields((INetSerializable) this.defined, (INetSerializable) this.uid);

    private Farmer getFarmer(long uid)
    {
      foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
      {
        if (onlineFarmer.UniqueMultiplayerID == uid)
          return onlineFarmer;
      }
      return (Farmer) null;
    }

    public NetFarmerRef Delayed(bool interpolationWait)
    {
      this.defined.Interpolated(false, interpolationWait);
      this.uid.Interpolated(false, interpolationWait);
      return this;
    }

    public IEnumerator<Farmer> GetEnumerator()
    {
      if ((bool) (NetFieldBase<bool, NetBool>) this.defined)
        yield return this.Value;
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public static implicit operator Farmer(NetFarmerRef farmerRef) => farmerRef.Value;
  }
}
