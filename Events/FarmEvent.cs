// Decompiled with JetBrains decompiler
// Type: StardewValley.Events.FarmEvent
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;

namespace StardewValley.Events
{
  public interface FarmEvent : INetObject<NetFields>
  {
    /// <summary>
    /// return true if the event wasn't able to set up and should be skipped
    /// </summary>
    bool setUp();

    bool tickUpdate(GameTime time);

    void draw(SpriteBatch b);

    void drawAboveEverything(SpriteBatch b);

    void makeChangesToLocation();
  }
}
