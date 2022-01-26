// Decompiled with JetBrains decompiler
// Type: StardewValley.ReachMineFloorObjective
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System;
using System.Collections.Generic;

namespace StardewValley
{
  public class ReachMineFloorObjective : OrderObjective
  {
    public NetBool skullCave = new NetBool(false);

    public override void InitializeNetFields()
    {
      base.InitializeNetFields();
      this.NetFields.AddFields((INetSerializable) this.skullCave);
    }

    public override void Load(SpecialOrder order, Dictionary<string, string> data)
    {
      base.Load(order, data);
      if (!data.ContainsKey("SkullCave") || !(data["SkullCave"].ToLowerInvariant() == "true"))
        return;
      this.skullCave.Value = true;
    }

    protected override void _Register()
    {
      base._Register();
      this._order.onMineFloorReached += new Action<Farmer, int>(this.OnNewValue);
    }

    protected override void _Unregister()
    {
      base._Unregister();
      this._order.onMineFloorReached -= new Action<Farmer, int>(this.OnNewValue);
    }

    public virtual void OnNewValue(Farmer who, int new_value)
    {
      if (this.skullCave.Value)
        new_value -= 120;
      else if (new_value > 120)
        return;
      if (new_value <= 0)
        return;
      this.SetCount(Math.Min(Math.Max(new_value, this.currentCount.Value), this.GetMaxCount()));
    }
  }
}
