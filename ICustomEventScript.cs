// Decompiled with JetBrains decompiler
// Type: StardewValley.ICustomEventScript
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StardewValley
{
  public interface ICustomEventScript
  {
    bool update(GameTime time, Event e);

    void draw(SpriteBatch b);

    void drawAboveAlwaysFront(SpriteBatch b);
  }
}
