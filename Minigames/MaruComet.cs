// Decompiled with JetBrains decompiler
// Type: StardewValley.Minigames.MaruComet
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;

namespace StardewValley.Minigames
{
  public class MaruComet : IMinigame
  {
    private const int telescopeCircleWidth = 143;
    private const int flybyRepeater = 200;
    private const float flybySpeed = 0.8f;
    private LocalizedContentManager content;
    private Vector2 centerOfScreen;
    private Vector2 cometColorOrigin;
    private Texture2D cometTexture;
    private List<Vector2> flybys = new List<Vector2>();
    private List<Vector2> flybysClose = new List<Vector2>();
    private List<Vector2> flybysFar = new List<Vector2>();
    private string currentString = "";
    private int zoom;
    private int flybyTimer;
    private int totalTimer;
    private int currentStringCharacter;
    private int characterAdvanceTimer;
    private float fade = 1f;

    public MaruComet()
    {
      this.zoom = 4;
      this.content = Game1.content.CreateTemporary();
      this.cometTexture = this.content.Load<Texture2D>("Minigames\\MaruComet");
      this.changeScreenSize();
    }

    public void changeScreenSize()
    {
      float num = 1f / Game1.options.zoomLevel;
      this.centerOfScreen = num * new Vector2((float) (Game1.game1.localMultiplayerWindow.Width / 2), (float) (Game1.game1.localMultiplayerWindow.Height / 2));
      this.centerOfScreen.X = (float) (int) this.centerOfScreen.X;
      this.centerOfScreen.Y = (float) (int) this.centerOfScreen.Y;
      this.cometColorOrigin = this.centerOfScreen + num * new Vector2((float) (-71 * this.zoom), (float) (71 * this.zoom));
    }

    public bool doMainGameUpdates() => false;

    public bool tick(GameTime time)
    {
      this.flybyTimer -= time.ElapsedGameTime.Milliseconds;
      if ((double) this.fade > 0.0)
        this.fade -= (float) time.ElapsedGameTime.Milliseconds * (1f / 1000f);
      if (this.flybyTimer <= 0)
      {
        this.flybyTimer = 200;
        bool flag = Game1.random.NextDouble() < 0.5;
        this.flybys.Add(new Vector2(flag ? (float) Game1.random.Next(143 * this.zoom) : (float) (-8 * this.zoom), flag ? (float) (8 * this.zoom) : (float) -Game1.random.Next(143 * this.zoom)));
        this.flybysClose.Add(new Vector2(flag ? (float) Game1.random.Next(143 * this.zoom) : (float) (-8 * this.zoom), flag ? (float) (8 * this.zoom) : (float) -Game1.random.Next(143 * this.zoom)));
        this.flybysFar.Add(new Vector2(flag ? (float) Game1.random.Next(143 * this.zoom) : (float) (-8 * this.zoom), flag ? (float) (8 * this.zoom) : (float) -Game1.random.Next(143 * this.zoom)));
      }
      TimeSpan elapsedGameTime;
      for (int index1 = this.flybys.Count - 1; index1 >= 0; --index1)
      {
        List<Vector2> flybys = this.flybys;
        int index2 = index1;
        double x1 = (double) this.flybys[index1].X;
        elapsedGameTime = time.ElapsedGameTime;
        double num1 = 0.800000011920929 * (double) elapsedGameTime.Milliseconds;
        double x2 = x1 + num1;
        double y1 = (double) this.flybys[index1].Y;
        elapsedGameTime = time.ElapsedGameTime;
        double num2 = 0.800000011920929 * (double) elapsedGameTime.Milliseconds;
        double y2 = y1 - num2;
        Vector2 vector2 = new Vector2((float) x2, (float) y2);
        flybys[index2] = vector2;
        if ((double) this.cometColorOrigin.Y + (double) this.flybys[index1].Y < (double) this.centerOfScreen.Y - (double) (143 * this.zoom / 2))
          this.flybys.RemoveAt(index1);
      }
      for (int index3 = this.flybysClose.Count - 1; index3 >= 0; --index3)
      {
        List<Vector2> flybysClose = this.flybysClose;
        int index4 = index3;
        double x3 = (double) this.flybysClose[index3].X;
        elapsedGameTime = time.ElapsedGameTime;
        double num3 = 0.800000011920929 * (double) elapsedGameTime.Milliseconds * 1.5;
        double x4 = x3 + num3;
        double y3 = (double) this.flybysClose[index3].Y;
        elapsedGameTime = time.ElapsedGameTime;
        double num4 = 0.800000011920929 * (double) elapsedGameTime.Milliseconds * 1.5;
        double y4 = y3 - num4;
        Vector2 vector2 = new Vector2((float) x4, (float) y4);
        flybysClose[index4] = vector2;
        if ((double) this.cometColorOrigin.Y + (double) this.flybysClose[index3].Y < (double) this.centerOfScreen.Y - (double) (143 * this.zoom / 2))
          this.flybysClose.RemoveAt(index3);
      }
      for (int index5 = this.flybysFar.Count - 1; index5 >= 0; --index5)
      {
        List<Vector2> flybysFar = this.flybysFar;
        int index6 = index5;
        double x5 = (double) this.flybysFar[index5].X;
        elapsedGameTime = time.ElapsedGameTime;
        double num5 = 0.800000011920929 * (double) elapsedGameTime.Milliseconds * 0.5;
        double x6 = x5 + num5;
        double y5 = (double) this.flybysFar[index5].Y;
        elapsedGameTime = time.ElapsedGameTime;
        double num6 = 0.800000011920929 * (double) elapsedGameTime.Milliseconds * 0.5;
        double y6 = y5 - num6;
        Vector2 vector2 = new Vector2((float) x6, (float) y6);
        flybysFar[index6] = vector2;
        if ((double) this.cometColorOrigin.Y + (double) this.flybysFar[index5].Y < (double) this.centerOfScreen.Y - (double) (143 * this.zoom / 2))
          this.flybysFar.RemoveAt(index5);
      }
      int totalTimer = this.totalTimer;
      elapsedGameTime = time.ElapsedGameTime;
      int milliseconds1 = elapsedGameTime.Milliseconds;
      this.totalTimer = totalTimer + milliseconds1;
      if (this.totalTimer >= 28000)
      {
        if (!this.currentString.Equals(Game1.content.LoadString("Strings\\Events:Maru_comet5")))
        {
          this.currentStringCharacter = 0;
          this.currentString = Game1.content.LoadString("Strings\\Events:Maru_comet5");
        }
      }
      else if (this.totalTimer >= 25000)
      {
        if (!this.currentString.Equals(Game1.content.LoadString("Strings\\Events:Maru_comet4")))
        {
          this.currentStringCharacter = 0;
          this.currentString = Game1.content.LoadString("Strings\\Events:Maru_comet4");
        }
      }
      else if (this.totalTimer >= 20000)
      {
        if (!this.currentString.Equals(Game1.content.LoadString("Strings\\Events:Maru_comet3")))
        {
          this.currentStringCharacter = 0;
          this.currentString = Game1.content.LoadString("Strings\\Events:Maru_comet3");
        }
      }
      else if (this.totalTimer >= 16000)
      {
        if (!this.currentString.Equals(Game1.content.LoadString("Strings\\Events:Maru_comet2")))
        {
          this.currentStringCharacter = 0;
          this.currentString = Game1.content.LoadString("Strings\\Events:Maru_comet2");
        }
      }
      else if (this.totalTimer >= 10000 && !this.currentString.Equals(Game1.content.LoadString("Strings\\Events:Maru_comet1")))
      {
        this.currentStringCharacter = 0;
        this.currentString = Game1.content.LoadString("Strings\\Events:Maru_comet1");
      }
      int characterAdvanceTimer = this.characterAdvanceTimer;
      elapsedGameTime = time.ElapsedGameTime;
      int milliseconds2 = elapsedGameTime.Milliseconds;
      this.characterAdvanceTimer = characterAdvanceTimer + milliseconds2;
      if (this.characterAdvanceTimer > 30)
      {
        ++this.currentStringCharacter;
        this.characterAdvanceTimer = 0;
      }
      if (this.totalTimer >= 35000)
      {
        double fade = (double) this.fade;
        elapsedGameTime = time.ElapsedGameTime;
        double num = (double) elapsedGameTime.Milliseconds * (1.0 / 500.0);
        this.fade = (float) (fade + num);
        if ((double) this.fade >= 1.0)
        {
          if (Game1.currentLocation.currentEvent != null)
            ++Game1.currentLocation.currentEvent.CurrentCommand;
          return true;
        }
      }
      return false;
    }

    public void draw(SpriteBatch b)
    {
      b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointWrap);
      SpriteBatch spriteBatch1 = b;
      Texture2D cometTexture1 = this.cometTexture;
      Vector2 cometColorOrigin1 = this.cometColorOrigin;
      TimeSpan totalGameTime1 = Game1.currentGameTime.TotalGameTime;
      double x1 = (double) (int) (totalGameTime1.TotalMilliseconds / 2.0 % 808.0);
      totalGameTime1 = Game1.currentGameTime.TotalGameTime;
      double y1 = (double) -(int) (totalGameTime1.TotalMilliseconds / 2.0 % 808.0);
      Vector2 vector2_1 = new Vector2((float) x1, (float) y1);
      Vector2 position1 = cometColorOrigin1 + vector2_1;
      Rectangle? sourceRectangle1 = new Rectangle?(new Rectangle(247, 0, 265, 240));
      Color white1 = Color.White;
      Vector2 origin1 = new Vector2(265f, 0.0f);
      double zoom1 = (double) this.zoom;
      spriteBatch1.Draw(cometTexture1, position1, sourceRectangle1, white1, 0.0f, origin1, (float) zoom1, SpriteEffects.None, 0.1f);
      SpriteBatch spriteBatch2 = b;
      Texture2D cometTexture2 = this.cometTexture;
      Vector2 cometColorOrigin2 = this.cometColorOrigin;
      TimeSpan totalGameTime2 = Game1.currentGameTime.TotalGameTime;
      double x2 = (double) ((int) (totalGameTime2.TotalMilliseconds / 2.0 % 808.0) + 808);
      totalGameTime2 = Game1.currentGameTime.TotalGameTime;
      double y2 = (double) (-(int) (totalGameTime2.TotalMilliseconds / 2.0 % 808.0) - 808);
      Vector2 vector2_2 = new Vector2((float) x2, (float) y2);
      Vector2 position2 = cometColorOrigin2 + vector2_2;
      Rectangle? sourceRectangle2 = new Rectangle?(new Rectangle(247, 0, 265, 240));
      Color white2 = Color.White;
      Vector2 origin2 = new Vector2(265f, 0.0f);
      double zoom2 = (double) this.zoom;
      spriteBatch2.Draw(cometTexture2, position2, sourceRectangle2, white2, 0.0f, origin2, (float) zoom2, SpriteEffects.None, 0.1f);
      b.Draw(this.cometTexture, this.centerOfScreen + new Vector2(-71f, -71f) * (float) this.zoom, new Rectangle?(new Rectangle((int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 300.0 / 100.0) * 143, 240, 143, 143)), Color.White, 0.0f, Vector2.Zero, (float) this.zoom, SpriteEffects.None, 0.2f);
      foreach (Vector2 flyby in this.flybys)
        b.Draw(this.cometTexture, this.cometColorOrigin + flyby, new Rectangle?(new Rectangle(0, 0, 8, 8)), Color.White * 0.4f, 0.0f, Vector2.Zero, (float) this.zoom, SpriteEffects.None, 0.24f);
      foreach (Vector2 vector2_3 in this.flybysClose)
        b.Draw(this.cometTexture, this.cometColorOrigin + vector2_3, new Rectangle?(new Rectangle(0, 0, 8, 8)), Color.White * 0.4f, 0.0f, Vector2.Zero, (float) (this.zoom + 1), SpriteEffects.None, 0.24f);
      foreach (Vector2 vector2_4 in this.flybysFar)
        b.Draw(this.cometTexture, this.cometColorOrigin + vector2_4, new Rectangle?(new Rectangle(0, 0, 8, 8)), Color.White * 0.4f, 0.0f, Vector2.Zero, (float) (this.zoom - 1), SpriteEffects.None, 0.24f);
      b.Draw(this.cometTexture, this.centerOfScreen + new Vector2(-71f, -71f) * (float) this.zoom, new Rectangle?(new Rectangle(0, 97, 143, 143)), Color.White, 0.0f, Vector2.Zero, (float) this.zoom, SpriteEffects.None, 0.3f);
      b.Draw(Game1.staminaRect, new Rectangle(0, 0, (int) this.centerOfScreen.X - 71 * this.zoom, Game1.graphics.GraphicsDevice.Viewport.Height), new Rectangle?(Game1.staminaRect.Bounds), Color.Black, 0.0f, Vector2.Zero, SpriteEffects.None, 0.96f);
      SpriteBatch spriteBatch3 = b;
      Texture2D staminaRect1 = Game1.staminaRect;
      Viewport viewport = Game1.graphics.GraphicsDevice.Viewport;
      Rectangle destinationRectangle1 = new Rectangle(0, 0, viewport.Width, (int) this.centerOfScreen.Y - 71 * this.zoom);
      Rectangle? sourceRectangle3 = new Rectangle?(Game1.staminaRect.Bounds);
      Color black1 = Color.Black;
      Vector2 zero1 = Vector2.Zero;
      spriteBatch3.Draw(staminaRect1, destinationRectangle1, sourceRectangle3, black1, 0.0f, zero1, SpriteEffects.None, 0.96f);
      SpriteBatch spriteBatch4 = b;
      Texture2D staminaRect2 = Game1.staminaRect;
      int x3 = (int) this.centerOfScreen.X + 71 * this.zoom;
      viewport = Game1.graphics.GraphicsDevice.Viewport;
      int width = viewport.Width - ((int) this.centerOfScreen.X + 71 * this.zoom);
      int height = Game1.graphics.GraphicsDevice.Viewport.Height;
      Rectangle destinationRectangle2 = new Rectangle(x3, 0, width, height);
      Rectangle? sourceRectangle4 = new Rectangle?(Game1.staminaRect.Bounds);
      Color black2 = Color.Black;
      Vector2 zero2 = Vector2.Zero;
      spriteBatch4.Draw(staminaRect2, destinationRectangle2, sourceRectangle4, black2, 0.0f, zero2, SpriteEffects.None, 0.96f);
      b.Draw(Game1.staminaRect, new Rectangle((int) this.centerOfScreen.X - 71 * this.zoom, (int) this.centerOfScreen.Y + 71 * this.zoom, Game1.graphics.GraphicsDevice.Viewport.Width, Game1.graphics.GraphicsDevice.Viewport.Height - ((int) this.centerOfScreen.Y + 71 * this.zoom)), new Rectangle?(Game1.staminaRect.Bounds), Color.Black, 0.0f, Vector2.Zero, SpriteEffects.None, 0.96f);
      float heightOfString = (float) SpriteText.getHeightOfString(this.currentString, Game1.game1.localMultiplayerWindow.Width);
      float y3 = (float) ((int) this.centerOfScreen.Y + 79 * this.zoom);
      if ((double) y3 + (double) heightOfString > (double) Game1.viewport.Height)
        y3 += (float) Game1.viewport.Height - (y3 + heightOfString);
      SpriteText.drawStringHorizontallyCenteredAt(b, this.currentString, (int) this.centerOfScreen.X, (int) y3, this.currentStringCharacter, height: 99999, layerDepth: 0.99f, color: 3, maxWidth: Game1.game1.localMultiplayerWindow.Width);
      b.Draw(Game1.staminaRect, new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), new Rectangle?(Game1.staminaRect.Bounds), Color.Black * this.fade, 0.0f, Vector2.Zero, SpriteEffects.None, 1f);
      b.End();
    }

    public void leftClickHeld(int x, int y)
    {
    }

    public string minigameId() => (string) null;

    public bool overrideFreeMouseMovement() => Game1.options.SnappyMenus;

    public void receiveEventPoke(int data)
    {
    }

    public void receiveKeyPress(Keys k)
    {
    }

    public void receiveKeyRelease(Keys k)
    {
    }

    public void receiveLeftClick(int x, int y, bool playSound = true)
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

    public void unload() => this.content.Unload();

    public bool forceQuit() => false;
  }
}
