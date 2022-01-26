// Decompiled with JetBrains decompiler
// Type: StardewValley.GemsReward
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System.Collections.Generic;

namespace StardewValley
{
  public class GemsReward : OrderReward
  {
    public NetInt amount = new NetInt(0);

    public override void InitializeNetFields()
    {
      base.InitializeNetFields();
      this.NetFields.AddFields((INetSerializable) this.amount);
    }

    public override void Load(SpecialOrder order, Dictionary<string, string> data) => this.amount.Value = int.Parse(order.Parse(data["Amount"]));

    public override void Grant() => Game1.player.QiGems += this.amount.Value;
  }
}
