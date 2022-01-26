// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.ICreditsBlock
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework.Graphics;

namespace StardewValley.Menus
{
  public abstract class ICreditsBlock
  {
    public virtual void draw(int topLeftX, int topLeftY, int widthToOccupy, SpriteBatch b)
    {
    }

    public virtual int getHeight(int maxWidth) => 0;

    public virtual void hovered()
    {
    }

    public virtual void clicked()
    {
    }
  }
}
