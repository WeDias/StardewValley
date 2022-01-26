// Decompiled with JetBrains decompiler
// Type: StardewValley.JKScoreObjective
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using System;

namespace StardewValley
{
  public class JKScoreObjective : OrderObjective
  {
    protected override void _Register()
    {
      base._Register();
      this._order.onJKScoreAchieved += new Action<Farmer, int>(this.OnNewValue);
    }

    protected override void _Unregister()
    {
      base._Unregister();
      this._order.onJKScoreAchieved -= new Action<Farmer, int>(this.OnNewValue);
    }

    public virtual void OnNewValue(Farmer who, int new_value) => this.SetCount(Math.Min(Math.Max(new_value, this.currentCount.Value), this.GetMaxCount()));
  }
}
