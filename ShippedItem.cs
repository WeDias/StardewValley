// Decompiled with JetBrains decompiler
// Type: StardewValley.ShippedItem
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

namespace StardewValley
{
  internal struct ShippedItem
  {
    public int index;
    public int price;
    public string name;

    public ShippedItem(int index, int price, string name)
    {
      this.index = index;
      this.price = price;
      this.name = name;
    }
  }
}
