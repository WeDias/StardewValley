// Decompiled with JetBrains decompiler
// Type: StardewValley.ISalable
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StardewValley
{
  public interface ISalable
  {
    string DisplayName { get; }

    bool ShouldDrawIcon();

    void drawInMenu(
      SpriteBatch spriteBatch,
      Vector2 location,
      float scaleSize,
      float transparency,
      float layerDepth,
      StackDrawType drawStackNumber,
      Color color,
      bool drawShadow);

    string Name { get; }

    string getDescription();

    int maximumStackSize();

    int addToStack(Item stack);

    int Stack { get; set; }

    int salePrice();

    bool actionWhenPurchased();

    bool canStackWith(ISalable other);

    bool CanBuyItem(Farmer farmer);

    bool IsInfiniteStock();

    ISalable GetSalableInstance();
  }
}
