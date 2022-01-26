// Decompiled with JetBrains decompiler
// Type: StardewValley.ShipObjective
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley
{
  public class ShipObjective : OrderObjective
  {
    [XmlElement("acceptableContextTagSets")]
    public NetStringList acceptableContextTagSets = new NetStringList();
    [XmlElement("useShipmentValue")]
    public NetBool useShipmentValue = new NetBool();

    public override void Load(SpecialOrder order, Dictionary<string, string> data)
    {
      if (data.ContainsKey("AcceptedContextTags"))
        this.acceptableContextTagSets.Add(order.Parse(data["AcceptedContextTags"]));
      if (!data.ContainsKey("UseShipmentValue") || !(data["UseShipmentValue"].ToLowerInvariant().Trim() == "true"))
        return;
      this.useShipmentValue.Value = true;
    }

    public override void InitializeNetFields()
    {
      base.InitializeNetFields();
      this.NetFields.AddFields((INetSerializable) this.acceptableContextTagSets, (INetSerializable) this.useShipmentValue);
    }

    protected override void _Register()
    {
      base._Register();
      this._order.onItemShipped += new Action<Farmer, Item, int>(this.OnItemShipped);
    }

    protected override void _Unregister()
    {
      base._Unregister();
      this._order.onItemShipped -= new Action<Farmer, Item, int>(this.OnItemShipped);
    }

    public virtual void OnItemShipped(Farmer farmer, Item item, int shipped_price)
    {
      foreach (string acceptableContextTagSet in (NetList<string, NetString>) this.acceptableContextTagSets)
      {
        bool flag1 = false;
        foreach (string str1 in acceptableContextTagSet.Split(','))
        {
          bool flag2 = false;
          foreach (string str2 in str1.Split('/'))
          {
            if (item.HasContextTag(str2.Trim()))
            {
              flag2 = true;
              break;
            }
          }
          if (!flag2)
            flag1 = true;
        }
        if (!flag1)
        {
          if (this.useShipmentValue.Value)
          {
            this.IncrementCount(shipped_price);
            break;
          }
          this.IncrementCount(item.Stack);
          break;
        }
      }
    }
  }
}
