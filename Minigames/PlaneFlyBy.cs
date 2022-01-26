// Decompiled with JetBrains decompiler
// Type: StardewValley.Minigames.PlaneFlyBy
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
  public class PlaneFlyBy : IMinigame
  {
    public const float backGroundSpeed = 0.25f;
    public const float robotSpeed = 1f;
    public const int skyLength = 2560;
    public int millisecondsSinceStart;
    public int backgroundPosition = (int) ((double) Game1.game1.localMultiplayerWindow.Height / (double) Game1.options.zoomLevel) - 2560;
    public int smokeTimer = 500;
    public Vector2 robotPosition = new Vector2((float) Game1.game1.localMultiplayerWindow.Width, (float) (Game1.game1.localMultiplayerWindow.Height / 2)) * 1f / Game1.options.zoomLevel;
    public List<TemporaryAnimatedSprite> tempSprites = new List<TemporaryAnimatedSprite>();

    public bool overrideFreeMouseMovement() => Game1.options.SnappyMenus;

    public bool tick(GameTime time)
    {
      this.millisecondsSinceStart += time.ElapsedGameTime.Milliseconds;
      this.robotPosition.X -= (float) (1.0 * (double) time.ElapsedGameTime.Milliseconds / 4.0);
      this.smokeTimer -= time.ElapsedGameTime.Milliseconds;
      if (this.smokeTimer <= 0)
      {
        this.smokeTimer = 100;
        this.tempSprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(173, 1828, 15, 20), 1500f, 2, 0, this.robotPosition + new Vector2(68f, -24f), false, false)
        {
          motion = new Vector2(0.0f, 0.1f),
          scale = 4f,
          scaleChange = 1f / 500f,
          alphaFade = 1f / 400f,
          rotation = -1.570796f
        });
      }
      for (int index = this.tempSprites.Count - 1; index >= 0; --index)
      {
        if (this.tempSprites[index].update(time))
          this.tempSprites.RemoveAt(index);
      }
      if ((double) this.robotPosition.X < (double) sbyte.MinValue && !Game1.globalFade)
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
      this.robotPosition.X = -1000f;
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
      b.Draw(Game1.mouseCursors, this.robotPosition, new Rectangle?(new Rectangle(222 + this.millisecondsSinceStart / 50 % 2 * 20, 1890, 20, 9)), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
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

    public bool forceQuit() => false;
  }
}
