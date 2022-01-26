// Decompiled with JetBrains decompiler
// Type: StardewValley.OrderReward
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley
{
  public class OrderReward : INetObject<NetFields>
  {
    [XmlIgnore]
    public NetFields NetFields { get; } = new NetFields();

    public OrderReward() => this.InitializeNetFields();

    public virtual void InitializeNetFields()
    {
    }

    public virtual void Grant()
    {
    }

    public virtual void Load(SpecialOrder order, Dictionary<string, string> data)
    {
    }
  }
}
