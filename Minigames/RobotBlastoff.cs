// Decompiled with JetBrains decompiler
// Type: StardewValley.Minigames.RobotBlastoff
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace StardewValley.Minigames
{
  public class RobotBlastoff : IMinigame
  {
    public const float backGroundSpeed = 0.25f;
    public const float robotSpeed = 0.3f;
    public const int skyLength = 2560;
    public int millisecondsSinceStart;
    public int backgroundPosition = (int) ((double) Game1.game1.localMultiplayerWindow.Height / (double) Game1.options.zoomLevel) - 2560;
    public int smokeTimer = 500;
    public Vector2 robotPosition = new Vector2((float) (Game1.game1.localMultiplayerWindow.Width / 2) / Game1.options.zoomLevel, (float) Game1.game1.localMultiplayerWindow.Height / Game1.options.zoomLevel);
    public List<TemporaryAnimatedSprite> tempSprites = new List<TemporaryAnimatedSprite>();

    public bool overrideFreeMouseMovement() => Game1.options.SnappyMenus;

    public bool tick(GameTime time)
    {
      this.millisecondsSinceStart += time.ElapsedGameTime.Milliseconds;
      float num = (float) (1.35000002384186 - 0.850000023841858 * (5.0 / (double) Math.Max(5f, this.robotPosition.Y / 20f)));
      this.backgroundPosition += (int) (0.25 * (double) time.ElapsedGameTime.Milliseconds * (double) num) / 2;
      this.robotPosition.Y -= (float) (0.300000011920929 * (double) time.ElapsedGameTime.Milliseconds / 4.0);
      this.smokeTimer -= time.ElapsedGameTime.Milliseconds;
      if (this.smokeTimer <= 0)
      {
        this.smokeTimer = 350;
        this.tempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(143, 1828, 15, 20), 1500f, 4, 0, this.robotPosition + new Vector2(0.0f, 72f), false, false)
        {
          motion = new Vector2(0.0f, -0.9f),
          acceleration = new Vector2(-1f / 1000f, 3f / 500f),
          scale = 4f,
          scaleChange = 1f / 500f,
          alphaFade = 1f / 400f
        });
      }
      for (int index = this.tempSprites.Count - 1; index >= 0; --index)
      {
        if (this.tempSprites[index].update(time))
          this.tempSprites.RemoveAt(index);
      }
      if ((double) this.robotPosition.Y < 0.0 && Game1.random.NextDouble() < 0.005)
        this.tempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(256, 1680, 16, 16), 80f, 5, 0, new Vector2((float) Game1.random.Next(Game1.graphics.GraphicsDevice.Viewport.Width), (float) Game1.random.Next(Game1.graphics.GraphicsDevice.Viewport.Height / 2)), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f)
        {
          motion = new Vector2(4f, 4f)
        });
      if ((double) this.robotPosition.Y < -512.0 && !Game1.globalFade)
        Game1.globalFadeToBlack(new Game1.afterFadeFunction(this.afterFade), 3f / 500f);
      return false;
    }

    public void afterFade()
    {
      Game1.currentMinigame = (IMinigame) null;
      Game1.globalFadeToClear();
      if (Game1.currentLocation.currentEvent == null)
        return;
      ++Game1.currentLocation.currentEvent.CurrentCommand;
      Game1.currentLocation.temporarySprites.Clear();
    }

    public bool forceQuit() => false;

    public void receiveLeftClick(int x, int y, bool playSound = true)
    {
    }

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
      if (k != Keys.Escape)
        return;
      this.robotPosition.Y = -1000f;
      this.tempSprites.Clear();
    }

    public void receiveKeyRelease(Keys k)
    {
    }

    public void draw(SpriteBatch b)
    {
      b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
      b.Draw(Game1.mouseCursors, new Rectangle(0, this.backgroundPosition, Game1.graphics.GraphicsDevice.Viewport.Width, 2560), new Rectangle?(new Rectangle(264, 1858, 1, 84)), Color.White);
      b.Draw(Game1.mouseCursors, new Vector2(0.0f, (float) this.backgroundPosition), new Rectangle?(new Rectangle(0, 1454, 639, 188)), Color.White * 0.5f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      b.Draw(Game1.mouseCursors, new Vector2(0.0f, (float) (this.backgroundPosition - 752)), new Rectangle?(new Rectangle(0, 1454, 639, 188)), Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      b.Draw(Game1.mouseCursors, new Vector2(0.0f, (float) (this.backgroundPosition - 1504)), new Rectangle?(new Rectangle(0, 1454, 639, 188)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      b.Draw(Game1.mouseCursors, new Vector2(0.0f, (float) (this.backgroundPosition - 2256)), new Rectangle?(new Rectangle(0, 1454, 639, 188)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      b.Draw(Game1.mouseCursors, this.robotPosition + new Vector2((float) Game1.random.Next(-1, 2), (float) Game1.random.Next(-1, 2)), new Rectangle?(new Rectangle(206 + this.millisecondsSinceStart / 50 % 4 * 15, 1827, 15, 27)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
      foreach (TemporaryAnimatedSprite tempSprite in this.tempSprites)
        tempSprite.draw(b, true);
      b.End();
    }

    public void changeScreenSize()
    {
      float num = 1f / Game1.options.zoomLevel;
      this.backgroundPosition = 2560 - (int) ((double) Game1.game1.localMultiplayerWindow.Height * (double) num);
      this.robotPosition = new Vector2((float) (Game1.game1.localMultiplayerWindow.Width / 2), (float) Game1.game1.localMultiplayerWindow.Height) * num;
    }

    public void unload()
    {
    }

    public void receiveEventPoke(int data) => throw new NotImplementedException();

    public string minigameId() => (string) null;

    public bool doMainGameUpdates() => false;
  }
}
