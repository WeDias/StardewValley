// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.ToolDescription
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

namespace StardewValley.Tools
{
  public struct ToolDescription
  {
    public byte index;
    public byte upgradeLevel;

    public ToolDescription(byte index, byte upgradeLevel)
    {
      this.index = index;
      this.upgradeLevel = upgradeLevel;
    }
  }
}
