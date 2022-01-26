// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.Stackable
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

namespace StardewValley.Tools
{
  public abstract class Stackable : Tool
  {
    private int numberInStack;

    public int NumberInStack
    {
      get => this.numberInStack;
      set => this.numberInStack = value;
    }

    public Stackable()
    {
    }

    public Stackable(
      string name,
      int upgradeLevel,
      int initialParentTileIndex,
      int indexOfMenuItemView,
      bool stackable)
      : base(name, upgradeLevel, initialParentTileIndex, indexOfMenuItemView, stackable)
    {
    }
  }
}
