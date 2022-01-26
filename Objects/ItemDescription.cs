// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.ItemDescription
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

namespace StardewValley.Objects
{
  public struct ItemDescription
  {
    public byte type;
    public int index;
    public int stack;

    public ItemDescription(byte type, int index, int stack)
    {
      this.type = type;
      this.index = index;
      this.stack = stack;
    }
  }
}
