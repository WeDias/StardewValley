// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.BundleIngredientDescription
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

namespace StardewValley.Menus
{
  public struct BundleIngredientDescription
  {
    public int index;
    public int stack;
    public int quality;
    public bool completed;

    public BundleIngredientDescription(int index, int stack, int quality, bool completed)
    {
      this.completed = completed;
      this.index = index;
      this.stack = stack;
      this.quality = quality;
    }
  }
}
