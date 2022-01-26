// Decompiled with JetBrains decompiler
// Type: StardewValley.Minigames.Test
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Objects;
using System.Collections.Generic;

namespace StardewValley.Minigames
{
  public class Test : IMinigame
  {
    public List<Wallpaper> wallpaper = new List<Wallpaper>();

    public Test()
    {
      for (int which = 0; which < 40; ++which)
        this.wallpaper.Add(new Wallpaper(which, true));
    }

    public bool overrideFreeMouseMovement() => Game1.options.SnappyMenus;

    public bool tick(GameTime time) => false;

    public void afterFade()
    {
    }

    public void receiveLeftClick(int x, int y, bool playSound = true) => Game1.currentMinigame = (IMinigame) null;

    public void leftClickHeld(int x, int y)
    {
    }

    public void receiveRightClick(int x, int y, bool playSound = true)
    {
    }

    public void releaseLeftClick(int x, int y)
    {
    }

    public void releaseRightClick(int x, int y)
    {
    }

    public void receiveKeyPress(Keys k)
    {
    }

    public void receiveKeyRelease(Keys k)
    {
    }

    public void draw(SpriteBatch b)
    {
      b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
      b.Draw(Game1.staminaRect, new Rectangle(0, 0, 2000, 2000), Color.White);
      Vector2 location = new Vector2(16f, 16f);
      for (int index = 0; index < this.wallpaper.Count; ++index)
      {
        this.wallpaper[index].drawInMenu(b, location, 1f);
        location.X += 128f;
        if ((double) location.X >= (double) (Game1.graphics.GraphicsDevice.Viewport.Width - 128))
        {
          location.X = 16f;
          location.Y += 128f;
        }
      }
      b.End();
    }

    public void changeScreenSize()
    {
    }

    public void unload()
    {
    }

    public void receiveEventPoke(int data)
    {
    }

    public string minigameId() => (string) null;

    public bool doMainGameUpdates() => false;

    public bool forceQuit() => true;
  }
}
