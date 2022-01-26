// Decompiled with JetBrains decompiler
// Type: StardewValley.Minigames.IMinigame
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StardewValley.Minigames
{
  public interface IMinigame
  {
    /// <summary>game tick for minigame</summary>
    /// <returns>true if finished</returns>
    bool tick(GameTime time);

    bool overrideFreeMouseMovement();

    bool doMainGameUpdates();

    void receiveLeftClick(int x, int y, bool playSound = true);

    void leftClickHeld(int x, int y);

    void receiveRightClick(int x, int y, bool playSound = true);

    void releaseLeftClick(int x, int y);

    void releaseRightClick(int x, int y);

    void receiveKeyPress(Keys k);

    void receiveKeyRelease(Keys k);

    void draw(SpriteBatch b);

    void changeScreenSize();

    void unload();

    void receiveEventPoke(int data);

    string minigameId();

    bool forceQuit();
  }
}
