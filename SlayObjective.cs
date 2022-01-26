// Decompiled with JetBrains decompiler
// Type: StardewValley.SlayObjective
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley
{
  public class SlayObjective : OrderObjective
  {
    [XmlElement("targetNames")]
    public NetStringList targetNames = new NetStringList();

    public override void InitializeNetFields()
    {
      base.InitializeNetFields();
      this.NetFields.AddFields((INetSerializable) this.targetNames);
    }

    public override void Load(SpecialOrder order, Dictionary<string, string> data)
    {
      base.Load(order, data);
      if (!data.ContainsKey("TargetName"))
        return;
      foreach (string str in order.Parse(data["TargetName"]).Split(','))
        this.targetNames.Add(str.Trim());
    }

    protected override void _Register()
    {
      base._Register();
      this._order.onMonsterSlain += new Action<Farmer, Monster>(this.OnMonsterSlain);
    }

    protected override void _Unregister()
    {
      base._Unregister();
      this._order.onMonsterSlain -= new Action<Farmer, Monster>(this.OnMonsterSlain);
    }

    public virtual void OnMonsterSlain(Farmer farmer, Monster monster)
    {
      foreach (string targetName in (NetList<string, NetString>) this.targetNames)
      {
        if (monster.Name.Contains(targetName))
        {
          this.IncrementCount(1);
          break;
        }
      }
    }
  }
}
