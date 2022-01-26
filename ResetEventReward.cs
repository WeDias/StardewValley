// Decompiled with JetBrains decompiler
// Type: StardewValley.ResetEventReward
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System;
using System.Collections.Generic;

namespace StardewValley
{
  public class ResetEventReward : OrderReward
  {
    public NetIntList resetEvents = new NetIntList();

    public override void InitializeNetFields()
    {
      base.InitializeNetFields();
      this.NetFields.AddFields((INetSerializable) this.resetEvents);
    }

    public override void Load(SpecialOrder order, Dictionary<string, string> data)
    {
      foreach (string str in order.Parse(data["ResetEvents"]).Split(' '))
        this.resetEvents.Add(Convert.ToInt32(str));
    }

    public override void Grant()
    {
      foreach (int resetEvent in (NetList<int, NetInt>) this.resetEvents)
        Game1.player.eventsSeen.Remove(resetEvent);
    }
  }
}
