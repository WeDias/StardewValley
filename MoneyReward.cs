// Decompiled with JetBrains decompiler
// Type: StardewValley.MoneyReward
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System.Collections.Generic;

namespace StardewValley
{
  public class MoneyReward : OrderReward
  {
    public NetInt amount = new NetInt(0);
    public NetFloat multiplier = new NetFloat(1f);

    public override void InitializeNetFields()
    {
      base.InitializeNetFields();
      this.NetFields.AddFields((INetSerializable) this.amount, (INetSerializable) this.multiplier);
    }

    public virtual int GetRewardMoneyAmount() => (int) ((double) this.amount.Value * (double) this.multiplier.Value);

    public override void Load(SpecialOrder order, Dictionary<string, string> data)
    {
      this.amount.Value = int.Parse(order.Parse(data["Amount"]));
      if (!data.ContainsKey("Multiplier"))
        return;
      this.multiplier.Value = float.Parse(order.Parse(data["Multiplier"]));
    }

    public override void Grant() => base.Grant();
  }
}
