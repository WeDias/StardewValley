// Decompiled with JetBrains decompiler
// Type: StardewValley.Quests.NetDescriptionElementList
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using System.Collections.Generic;

namespace StardewValley.Quests
{
  public class NetDescriptionElementList : NetList<DescriptionElement, NetDescriptionElementRef>
  {
    public NetDescriptionElementList()
    {
    }

    public NetDescriptionElementList(IEnumerable<DescriptionElement> values)
      : base(values)
    {
    }

    public NetDescriptionElementList(int capacity)
      : base(capacity)
    {
    }
  }
}
